using Microsoft.Extensions.Logging;

namespace One.More.Lib.For.MediatR.Test;

public class SpyLogger : ILogger
{
    private readonly List<string> _logs = new();
    public IReadOnlyList<string> Logs => _logs;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logs.Add(formatter(state, null));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}

public class SpyLogger<T> : SpyLogger, ILogger<T>;