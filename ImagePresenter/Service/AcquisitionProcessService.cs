using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace ImagePresenter;

public class AcquisitionProcessService : IAsyncDisposable
{
    public Process? Proc { get => _process; private set => _process = value; }
    private Process? _process = null;

    private MemoryMappedFile? _memoryMappedFile = null;

    #region User configuration
    public required string ProcessLocation { get; set; }
    public bool UseIsolatedTerminal { get; set; } = false;
    public int SharedMemorySize { get; set; } = 1024;
    public event EventHandler? OnProcessExit;
    #endregion // User configuration

    private readonly string _processMMF;
    private string _processMMFReadyness => _processMMF + "Ready";
    private readonly int _processPort;
    private readonly TimeSpan _processMMFReadynessTimeout;


    private EventWaitHandle? dataReady;
    private EventWaitHandle? dataProcessed;

    public AcquisitionProcessService()
    {
        _processMMF = Guid.NewGuid().ToString().Replace("-", "");
        _processPort = 1100;
        _processMMFReadynessTimeout = TimeSpan.FromSeconds(5);
    }

    private void _onProcessExit(object? sender, EventArgs e) => OnProcessExit?.Invoke(sender, e);
    
    public string ReadString()
    {
        var b = ReadBytes();
        return Encoding.UTF8.GetString(b, 0, b.Length).TrimEnd('\0');
    }

    public byte[] ReadBytes()
    {
        if (_memoryMappedFile is null)
            return Array.Empty<byte>();

        dataReady.WaitOne();

        using var accessor = _memoryMappedFile.CreateViewAccessor();

        byte[] buffer = new byte[SharedMemorySize];

        int bytesRead = accessor.ReadArray(0, buffer, 0, buffer.Length);

        dataProcessed.Set();

        Array.Resize(ref buffer, bytesRead);
        return buffer;
    }

    public void Start()
    {
        using var readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _processMMFReadyness);

        var job = CreateJobObject(IntPtr.Zero, null);
        var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
        info.BasicLimitInformation.LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;

        if (!SetInformationJobObject(job, JobObjectExtendedLimitInformation, ref info, (uint)Marshal.SizeOf(info)))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        var psi = new ProcessStartInfo(ProcessLocation)
        {
            UseShellExecute = !UseIsolatedTerminal,
            Arguments = $"{_processMMF} {SharedMemorySize} {_processPort}",
        };

        _process = Process.Start(psi)!;
        _process.EnableRaisingEvents = true;

        if (!AssignProcessToJobObject(job, _process.Handle))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        if (!readyEvent.WaitOne(_processMMFReadynessTimeout))
        {
            Console.Error.WriteLine("Timeout : Producer n’a pas signalé la création de la MMF.");
            Stop();
            return;
        }
        _process.Exited += _onProcessExit;
        _memoryMappedFile = MemoryMappedFile.OpenExisting(_processMMF);
        dataReady = EventWaitHandle.OpenExisting(_processMMF + "ProducerReady");
        dataProcessed = EventWaitHandle.OpenExisting(_processMMF + "ConsumerConsumed");
    }

    public void Stop()
    {
        if(Proc is not null){
            Proc.Exited -= _onProcessExit;
            Proc.Kill();
            Proc.WaitForExit();
            Proc.Dispose();
            Proc = null;
        }
        _memoryMappedFile?.Dispose();
        _memoryMappedFile = null;
    }

    public async Task StopAsync()
    {
        if(Proc is not null){
            Proc.Exited -= _onProcessExit;
            Proc.Kill();
            await Proc.WaitForExitAsync();
            Proc.Dispose();
            Proc = null;
        }
        _memoryMappedFile?.Dispose();
        _memoryMappedFile = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (Proc is null)
            return;

        Stop();
    }

    #region FFI
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string? lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetInformationJobObject(IntPtr hJob, int jobInfoClass, ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobInformation, uint cbJobInformationLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

    const int JobObjectExtendedLimitInformation = 9;
    const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;

    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }
    #endregion // FFI
}
