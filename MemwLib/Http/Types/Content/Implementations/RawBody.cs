using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content.Implementations;

/// <summary>Body implementation for raw string body.</summary>
[PublicAPI]
public class RawBody : IBody
{
    private string _content;

    /// <inheritdoc cref="IBody.ContentType"/>
    public string ContentType => "text/plain";
    
    /// <summary>Initialize a raw body instance with a string.</summary>
    /// <param name="content">The string to initialize the body with.</param>
    public RawBody(string content)
    {
        _content = content;
    }

    /// <inheritdoc cref="IBody.ParseImpl"/>
    public static IBody ParseImpl(string content) 
        => new RawBody(content);

    /// <inheritdoc cref="IBody.ToRaw"/>
    public string ToRaw() 
        => _content;
}