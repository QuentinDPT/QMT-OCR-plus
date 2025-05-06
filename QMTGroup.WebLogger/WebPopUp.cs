using Microsoft.Extensions.Logging;
using QMTGroup.Core;

namespace QMTGroup.WebLogger;

public class WebPopUp : IPopUpLogger
{
    private readonly string _categoryName;

    public WebPopUp(string categoryName = "popup")
    {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.WriteLine(formatter.Invoke(state, exception));
    }
}
