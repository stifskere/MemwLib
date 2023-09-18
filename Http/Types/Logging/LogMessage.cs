using JetBrains.Annotations;

namespace MemwLib.Http.Types.Logging;

[PublicAPI]
public class LogMessage
{
    public DateTimeOffset Date { get; init; } = DateTimeOffset.Now;
    public  LogType Type { get; init; }
    public string Message { get; init; }

    public LogMessage(LogType type, string message, DateTimeOffset? date = null)
    {
        Type = type;
        Message = message;
        if (date is { } d)
            Date = d;
    }
    
    public override string ToString()
        => $"[{Date:T}] [{Type.ToString()}]: {Message}";

    public static implicit operator string(LogMessage instance)
        => instance.ToString();
}

[PublicAPI]
public enum LogType
{
    Info,
    SuccessfulRequest,
    FailedRequest,
    Warning,
    Error
}