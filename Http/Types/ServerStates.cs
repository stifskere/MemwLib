namespace MemwLib.Http.Types;

/// <summary>Constant definition for the server state.</summary>
public enum ServerStates
{
    /// <summary>
    /// Use this constant when the server must be in production mode,
    /// this means no details on the development will be exposed in any manner.
    /// </summary>
    Production,
    
    /// <summary>
    /// Use this constant when the server must be in development mode,
    /// this means that details on the development such as errors will be exposed
    /// </summary>
    /// <remarks>
    /// For your own security don't leave this constant on production as it leaves
    /// details and hints for any third party to attack your server.
    /// </remarks>
    Development
}