using JetBrains.Annotations;

#if DEBUG

namespace MemwLib.Cron.Types.Jobs;

/// <summary>This class represents a cron job.</summary>
/// <typeparam name="TReturn">The type of parameter this job returns each time is ran.</typeparam>
/// <remarks>This class can't be constructed without using a CronManager.</remarks>
[PublicAPI]
public class ReturningCronJob<TReturn> : CronJob
{
    private string _cron;
    private Delegate _action;
    private object[] _parameters;
    private CancellationToken? _cancellationToken;
    
    /// <summary>Event fired when this cron is executed.</summary>
    public event CronExecutedEventDelegate<TReturn> CronExecuted = delegate {};

    internal ReturningCronJob(string cron, Delegate action, object[] parameters, CancellationToken? cancellationToken)
        : base(cron, action, parameters, cancellationToken)
    {
        _cron = cron;
        _action = action;
        _parameters = parameters;
        _cancellationToken = cancellationToken;
    }

    /// <inheritdoc cref="ActionTask"/>
    protected override async void ActionTask()
    {
        while (!_cancellationToken?.IsCancellationRequested ?? true)
        {
            await WaitForCron();
            
            if (!Activated)
                continue;

            CronExecuted((TReturn)_action.DynamicInvoke(_parameters)!);
            TimesExecuted++;
        }
    }
}

#endif