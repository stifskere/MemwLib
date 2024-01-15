using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Content;

namespace MemwLib.Http.Types.Configuration;

/// <summary>Request configuration to send a request to a server.</summary>
[PublicAPI]
public class HttpRequestConfig
{
    /// <summary>Where to send the request to</summary>
    public required string Url { get; init; }

    /// <summary>The request method type.</summary>
    /// <remarks>Flags are not supported, using them will throw an exception.</remarks>
    public RequestMethodType Method { get; init; } = RequestMethodType.Get;

    /// <summary>The headers for this request.</summary>
    public HeaderCollection Headers { get; init; } = new();

    /// <summary>The body of this request.</summary>
    public IBody? Body { get; init; }
}