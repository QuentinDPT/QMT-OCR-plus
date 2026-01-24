
namespace LuaOrchestrator;

public class LuaScriptLine
{
    public LuaScriptLine(int index, string rawLine)
    {
        LineIndex = index;
        Line = rawLine;
    }

    public string? Error { get => _error; set => _error = value; }
    private string? _error;

    public string? Warning { get => _warning; set => _warning = value; }
    private string? _warning = null;

    public string Line
    {
        get => _lineRaw;
        set
        {
            _lineRaw = value;
            _lineMinified = value.TrimStart();
            _executable = !string.IsNullOrWhiteSpace(_lineMinified) && !_lineMinified.StartsWith("--");
        }
    }
    private string _lineRaw = string.Empty;

    public int LineIndex
    {
        get => _lineIndex;
        set => _lineIndex = value;
    }
    private int _lineIndex;

    public string LineMinified => _lineMinified;
    private string _lineMinified = string.Empty;

    /// <summary>
    /// in ticks
    /// </summary>
    public long ExecutionDuration
    {
        get => _executionDuration;
        set => _executionDuration = value;
    }
    private long _executionDuration = -1;

    public bool Executable => _executable;
    private bool _executable;
}
