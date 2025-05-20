using Microsoft.Extensions.Logging;

namespace QMTGroup.Core
{
    public class MemoryLoggerStub : IMemoryLogger, IWebLogger, IDisposable
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => this;

        public void Clear()
        {
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string str = formatter.Invoke(state, exception);
            Console.WriteLine("STUB\t" + str);
        }
    }
}
