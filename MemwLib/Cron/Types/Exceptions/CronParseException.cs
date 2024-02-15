using JetBrains.Annotations;

#if DEBUG

namespace MemwLib.Cron.Types.Exceptions;

/// <summary>Exception thrown when there was an error while parsing a cron expression.</summary>
/// <remarks>This exception is only thrown internally.</remarks>
internal sealed class CronParseException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    /// <summary>The invalid cron exception that threw the error.</summary>
    [PublicAPI]
    public string Expression { get; }
    
    internal CronParseException(string expression, string message)
    {
        Expression = expression;
        Message = message;
    }
}

#endif