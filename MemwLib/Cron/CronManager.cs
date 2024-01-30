using JetBrains.Annotations;
using MemwLib.Cron.Types.Exceptions;
using MemwLib.Cron.Types.Jobs;

namespace MemwLib.Cron;

/// <summary>This class acts as a manager for all of your cron jobs.</summary>
[PublicAPI]
public class CronManager
{
    private CancellationToken? _cancellationToken;
    private Dictionary<string, object> _jobs = new();
    
    /// <summary>Constructs a new instance of a cron manager.</summary>
    /// <param name="cancellationToken">Cancellation token used to stop all the cron jobs externally.</param>
    public CronManager(CancellationToken? cancellationToken = null)
    {
        _cancellationToken = cancellationToken;
    }

    /// <summary>Adds a cron job to this manager.</summary>
    /// <param name="identifier">An identifier for this cron, if null won't be obtainable and must rely on return value.</param>
    /// <param name="cron">The cron expression, when to run the task.</param>
    /// <param name="action">The task itself, if the function is not void the result will not be used.</param>
    /// <param name="parameters">The handler parameters.</param>
    /// <exception cref="CronParseException">Thrown when there was an error while parsing the cron expression.</exception>
    /// <exception cref="CronAlreadyExistsException">Thrown when another occurrence of this identifier was found in this manager.</exception>
    /// <returns>The CronJob instance that represents this job.</returns>
    public CronJob AddJob(string? identifier, string cron, Delegate action, params object[] parameters)
    {
        CronJob job = new(cron, action, parameters, _cancellationToken);
        
        if (!string.IsNullOrEmpty(identifier))
            if (!_jobs.TryAdd(identifier, job))
                throw new CronAlreadyExistsException(identifier);
        
        return job;
    }

    /// <inheritdoc cref="AddJob"/>
    /// <typeparam name="TReturn">The type of return this handler has.</typeparam>
    /// <returns>A CronJob instance that holds an event for return values.</returns>
    public ReturningCronJob<TReturn> AddJob<TReturn>(string? identifier, string cron, Delegate action, params object[] parameters)
    {
        ReturningCronJob<TReturn> job = new(cron, action, parameters, _cancellationToken);
        
        if (!string.IsNullOrEmpty(identifier))
            if (!_jobs.TryAdd(identifier, job))
                throw new CronAlreadyExistsException(identifier);

        return job;
    }
}