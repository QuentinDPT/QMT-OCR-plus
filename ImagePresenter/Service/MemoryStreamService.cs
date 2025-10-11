using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace ImagePresenter;

public class MemoryStreamService
{
    private MemoryMappedFile _memoryMappedFile;

    private Process _process;

    private string _sharedMemoryFileName = "BonjourSharedMemory";
    private string _sharedMemoryReadynessName = "BonjourSharedMemoryReady";

    private void CreateProcess()
    {
        var job = CreateJobObject(IntPtr.Zero, null);
        var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
        info.BasicLimitInformation.LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;

        if (!SetInformationJobObject(job, JobObjectExtendedLimitInformation, ref info, (uint)Marshal.SizeOf(info)))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        var psi = new ProcessStartInfo(@"D:\Dev\PERSO\QMT-OCR-plus\ImageProducer\bin\Debug\net8.0\ImageProducer.exe")
        {
            UseShellExecute = true
        };
        _process = Process.Start(psi)!;

        if (!AssignProcessToJobObject(job, _process.Handle))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        Console.WriteLine("Producer lancé et attaché au job.");
    }


    public MemoryStreamService()
    { }

    public async Task<string> GetStreamAsync() 
    {
        if (_memoryMappedFile is null)
            return string.Empty;

        using var accessor = _memoryMappedFile.CreateViewAccessor();

        byte[] buffer = new byte[1024];

        int bytesRead = accessor.ReadArray(0, buffer, 0, buffer.Length);

        // Transforme en string
        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd('\0');

        return message;
    }

    public bool StartProcess()
    {
        using var readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _sharedMemoryReadynessName);
        CreateProcess();
        if (!readyEvent.WaitOne(TimeSpan.FromSeconds(5))){
            Console.WriteLine("Timeout : Producer n’a pas signalé la création de la MMF.");
            _process.Kill();
            return false;
        }
        _memoryMappedFile = MemoryMappedFile.OpenExisting(_sharedMemoryFileName);
        return true;
    }

    public bool StopProcess()
    {
        _process.Kill();
        _process.WaitForExit();
        return true;
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
