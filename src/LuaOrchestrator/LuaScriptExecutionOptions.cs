
namespace LuaOrchestrator;

public class LuaScriptExecutionOptions
{
    /// <summary>
    /// Time the line execution.
    /// </summary>
    public bool TimedScriptLine { get; init; } = true;

    /// <summary>
    /// Wait for the user approval to execute the next line of the script.
    /// </summary>
    public bool UserApproval { get; init; } = false;
}
