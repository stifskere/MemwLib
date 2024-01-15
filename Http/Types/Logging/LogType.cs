using JetBrains.Annotations;

namespace MemwLib.Http.Types.Logging;

/// <summary>The log type enum for ILogMessage implementations</summary>
[PublicAPI]
public enum LogType
{
    /// <summary>Defines an information log.</summary>
    Info,
    
    /// <summary>Defines a successful request log.</summary>
    SuccessfulRequest,
    
    /// <summary>Defines a failed request log.</summary>
    FailedRequest,
    
    /// <summary>Defines a warning log.</summary>
    Warning,
    
    /// <summary>Defines an error log.</summary>
    /// <remarks>This should always go together with a stack trace.</remarks>
    Error
}