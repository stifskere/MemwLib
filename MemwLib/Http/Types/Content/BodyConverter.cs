using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content;

/// <summary>
/// This class is intended to add interoperability
/// between strings and IBody implementations
/// </summary>
[PublicAPI]
public class BodyConverter
{
    /// <summary>The content type this BodyConverter is holding.</summary>
    public string ContentType { get; }
    
    /// <summary>The raw body contained by this converter.</summary>
    public string? RawBody { get; }

    /// <summary>Whether the current body is empty or not.</summary>
    public bool IsEmpty => string.IsNullOrEmpty(RawBody);

    /// <summary>Gets the length of the body in a raw format.</summary>
    public int Length => RawBody?.Length ?? 0;

    /// <summary>Returns an empty instance of a BodyConverter.</summary>
    public static BodyConverter Empty => new(string.Empty);
    
    // TODO: must read stream instead of string, I'm thinking of reading it till Content-Length or end of stream data.
    
    /// <summary>
    /// BodyConverter raw constructor,
    /// initializes this instance from a raw string.
    /// </summary>
    /// <param name="raw">The string that contains the body content.</param>
    public BodyConverter(string? raw)
    {
        RawBody = raw;
        ContentType = IsEmpty ? "none/none" : "text/plain";
    }

    /// <summary>
    /// BodyConverter instance constructor, initializes
    /// this instance from a IBody implementation instance.
    /// </summary>
    /// <param name="body">The body to convert from.</param>
    public BodyConverter(IBody? body)
    {
        RawBody = body?.ToRaw() ?? string.Empty;
        ContentType = body?.ContentType ?? "none/none";
    }
    
    /// <summary>Reads the current BodyConverter instance as a body instance.</summary>
    /// <typeparam name="TBody">The type of body to convert to.</typeparam>
    /// <returns>
    /// An instance of TBody based on this converter's
    /// raw string or null if there was no body in the first place.
    /// </returns>
    public TBody? ReadAs<TBody>() where TBody : IBody 
        => RawBody is not null ? IBody.Parse<TBody>(RawBody) : default;

    /// <summary>Tries to read the current body converter instance as a body instance.</summary>
    /// <param name="body">The result of this conversion.</param>
    /// <typeparam name="TBody">The type of body to convert to.</typeparam>
    /// <returns>A boolean instance whether the body could be converted or not.</returns>
    public bool TryReadAs<TBody>([NotNullWhen(true)] out TBody? body) where TBody : IBody
    {
        if (RawBody is not null) 
            return IBody.TryParse(RawBody, out body);
        
        body = default;
        return false;
    }

    /// <inheritdoc cref="Object.ToString"/>
    public override string ToString() 
        => RawBody ?? string.Empty;
}