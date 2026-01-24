namespace LuaOrchestrator.Error;

public record ErrorElement
{
    public required string Title { get; init; }

    public required int Line { get; init; }

    public string Severity { get; init; } = "ERROR";

    public string Description { get; init; } = "";
}
