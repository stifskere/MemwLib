using System.Text;
using JetBrains.Annotations;
using MemwLib.CoreUtils;

namespace MemwLib.Http.Types.Content.Common;

/// <summary>Body implementation for raw string body.</summary>
[PublicAPI]
public class StringBody : IBody
{
    private string _content;

    /// <inheritdoc cref="IBody.ContentType"/>
    public string ContentType => "text/plain";
    
    /// <summary>Initialize a raw body instance with a string.</summary>
    /// <param name="content">The string to initialize the body with.</param>
    public StringBody(string content)
    {
        _content = content;
    }

    /// <inheritdoc cref="IBody.ParseImpl"/>
    public static IBody ParseImpl(MemoryStream content) 
        => new StringBody(content.GetRaw());

    /// <inheritdoc cref="IBody.ToArray"/>
    public byte[] ToArray() 
        => Encoding.ASCII.GetBytes(_content);

    /// <inheritdoc cref="Object.ToString"/>
    public override string ToString() 
        => _content;

    /// <summary>Implicitly call StringBody.ToString()</summary>
    /// <param name="body">The right operand of this cast operation.</param>
    /// <returns>The same as StringBody.ToString()</returns>
    public static implicit operator string(StringBody body) 
        => body._content;
}