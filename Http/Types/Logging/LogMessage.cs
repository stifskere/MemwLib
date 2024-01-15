using JetBrains.Annotations;

namespace MemwLib.Http.Types.Logging;

/// <summary>Default log message implementation for HTTP server logs.</summary>
[PublicAPI]
public class LogMessage
{
    /// <summary>The date of the log, if unset the actual date.</summary>
    public DateTimeOffset Date { get; init; } = DateTimeOffset.Now;
    
    /// <summary>The log type as LogType enumerable.</summary>
    public LogType Type { get; init; }
    
    /// <summary>The message corresponding to the log.</summary>
    public string Message { get; init; }
    
    internal LogMessage(LogType type, string message, DateTimeOffset? date = null)
    {
        Type = type;
        Message = message;
        if (date is { } d)
            Date = d;
    }
    
    /// <summary>Prepares the class for direct STDOUT.</summary>
    /// <returns>The formatted log.</returns>
    public override string ToString()
        => $"[{Date:T}] [{Type.ToString()}]: {Message}";

    /// <summary>Implicit cast operator to String, runs the ToString() method implicitly.</summary>
    /// <param name="instance">The right operand to run the ToString() method from.</param>
    /// <returns>The result of the ToString() method from the passed instance.</returns>
    public static implicit operator string(LogMessage instance)
        => instance.ToString();
}