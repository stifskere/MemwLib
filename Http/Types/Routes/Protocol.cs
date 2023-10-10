using JetBrains.Annotations;

namespace MemwLib.Http.Types.Routes;

/// <summary>
/// An enumeration representing different network
/// protocols with their associated port numbers.
/// </summary>
[PublicAPI]
public enum Protocol
{
    /// <summary>The HTTPS protocol, using port 443.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Glossary/HTTPS"/>
    Https = 443,
    
    /// <summary>The HTTP protocol, using port 80.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Glossary/HTTP"/>
    Http = 80
}