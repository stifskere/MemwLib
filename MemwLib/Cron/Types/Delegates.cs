namespace MemwLib.Cron.Types;

/// <summary>Delegate used for the on cron run event in the CronJob class.</summary>
/// <typeparam name="TValue">The type of value the handler returns when executed.</typeparam>
public delegate void CronExecutedEventDelegate<in TValue>(TValue returnValue);