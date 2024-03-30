
#if DEBUG

namespace MemwLib.Cron.Types.Exceptions;

/// <summary>This exception is thrown when there is already a cron entry in this CronManager.</summary>
public class CronAlreadyExistsException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal CronAlreadyExistsException(string key)
    {
        Message = $"A cron with the key: \"{key}\" already exists in this manager.";
    }
}

#endif