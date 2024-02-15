using JetBrains.Annotations;
using MemwLib.Cron.Types.Exceptions;

#if DEBUG

namespace MemwLib.Cron.Types.Jobs;

/// <summary>This class represents a basic cron job.</summary>
[PublicAPI]
public class CronJob
{
    private string _cron;
    private Delegate _action;
    private object[] _parameters;
    private CancellationToken? _cancellationToken;
    
    internal Task Job { get; }

    /// <summary>
    /// Whether this job runs or not, setting this value to
    /// true will let the job run, otherwise it will wait until true again.
    /// </summary>
    public bool Activated { get; set; }

    /// <summary>The times this cron was executed.</summary>
    public int TimesExecuted { get; protected set; }

    internal CronJob(string cron, Delegate action, object[] parameters, CancellationToken? cancellationToken)
    {
        _cron = cron;
        _action = action;
        _parameters = parameters;
        _cancellationToken = cancellationToken;
        
        Job = Task.Run(ActionTask);
    }
    
    /// <summary>The task this cron is going to be doing.</summary>
    protected virtual async void ActionTask()
    {
        while (!_cancellationToken?.IsCancellationRequested ?? true)
        {
            await WaitForCron();
            
            if (!Activated)
                continue;

            _action.DynamicInvoke(_parameters);
            TimesExecuted++;
        }
    }
    
    /// <summary
    /// >Waits for the next occurrence of time of
    /// this specific cron in this instance.
    /// </summary>
    /// <returns>
    /// A task to await that will complete when
    /// the next occurrence of time happens.
    /// </returns>
    /// <exception cref="CronParseException">
    /// Thrown when there was an error
    /// while parsing the cron expression.
    /// </exception>
    protected Task WaitForCron()
    {
        string[] parts = _cron.Split(' ');

        if (parts.Length != 5)
            throw new CronParseException(_cron, "The cron format is invalid, it should be \"* * * * *\"");

        DateTime currentDate = DateTime.Now;

        DateTime next = new DateTime(
            ParseCronPart(parts[4], currentDate.Year, 2100, currentDate.Year, "year"),
            ParseCronPart(parts[3], 1, 12, currentDate.Month, "month"),
            ParseCronPart(parts[2], 1, DateTime.DaysInMonth(currentDate.Year, currentDate.Month), currentDate.Day, "day"),
            ParseCronPart(parts[1], 0, 23, currentDate.Hour, "hour"),
            ParseCronPart(parts[0], 0, 59, currentDate.Minute, "minute"),
            0
        );

        Console.WriteLine(next);

        return Task.Delay(TimeSpan.FromSeconds(10));

        
        
        int ParseCronPart(string part, int min, int max, int def, string identifier)
        {
            if (part == "*")
                return def;

            if (part.Contains('/'))
            {
                string[] partDivisionOp = part.Split('/');

                if (partDivisionOp.Length != 2)
                    throw new CronParseException(_cron, $"Value for {identifier} divider invalid, expected a conventional division of 2 parts.");

                int startValue = ParseCronPart(partDivisionOp[0], min, max, def, identifier);
                if (!int.TryParse(partDivisionOp[1], out int increment) || increment < 1)
                    throw new CronParseException(_cron, $"value for {identifier} right operand of divider invalid, expected a number right operand of division.");
                
                if (startValue + increment > max)
                    throw new CronParseException(_cron, $"The resulting value for {identifier} is bigger than the allowed: {max}");

                return startValue;
            }
            
            if (!int.TryParse(part, out int value))
                throw new CronParseException(_cron, $"Value for {identifier} invalid, got: {part}, expected: number or wildcards.");

            if (value < min || value > max)
                throw new CronParseException(_cron, $"Value for {identifier} is out of range for {identifier}, expected min: {min}, max: {max}, got: {value}");

            return value;
        }
    }
}

#endif