using QMTGroup.Core;
using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.Logging;

namespace QMTGroup.WebLogger;

public class WebLogger : IWebLogger, IMemoryLogger
{
    private readonly string _categoryName;
    private string _lastLog = string.Empty;
    private Stream _htmlResponseBody;

    public WebLogger(Stream htmlResponseBody, string categoryName = "simple log")
    {
        _categoryName = categoryName;
        _htmlResponseBody = htmlResponseBody;
    }

    public event PropertyChangedEventHandler? LogChanged;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public void Clear()
    {
        var logBytes = Encoding.UTF8.GetBytes("===CLEAR_INTERFACE===");
        _htmlResponseBody.Write(logBytes, 0, logBytes.Length);
        _htmlResponseBody.Flush();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _lastLog = formatter.Invoke(state, exception).Replace("\n", "<br/>");
        var logBytes = Encoding.UTF8.GetBytes($"{{\"type\":\"log\",\"logType\":\"{logLevel}\",\"message\":\"{System.Net.WebUtility.HtmlEncode(_lastLog)}\"}},\n");

        Task.Run(async () =>
        {
            await _htmlResponseBody.WriteAsync(logBytes, 0, logBytes.Length);
            await _htmlResponseBody.FlushAsync();
        }).Wait();
    }
}
