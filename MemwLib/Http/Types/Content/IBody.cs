using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content;

/// <summary>Interface used to define which class behaves as a body.</summary>
[PublicAPI]
public interface IBody
{
    /// <summary>The content type of this body.</summary>
    public string ContentType { get; }
    
    /// <summary>
    /// This method implementation is used to parse
    /// the current body implementation from an string.
    /// </summary>
    /// <param name="content">The string content to parse from.</param>
    /// <returns>
    /// An instance of the current body
    /// implementation based on the provided string content.
    /// </returns>
    protected static abstract IBody ParseImpl(MemoryStream content);

    /// <summary>Convert the current body implementation to a raw string.</summary>
    /// <returns>A raw string representing the current body implementation.</returns>
    public byte[] ToArray();
    
    /// <summary>Parse some string to a body of the defined type.</summary>
    /// <param name="content">The raw content to parse from.</param>
    /// <typeparam name="TBody">The target type.</typeparam>
    /// <returns>
    /// An instance of the target type based in the
    /// provided raw string content.
    /// </returns>
    public static TBody Parse<TBody>(MemoryStream content) where TBody : IBody 
        => (TBody)TBody.ParseImpl(content);

    /// <summary>Tries to parse a raw string to a body.</summary>
    /// <param name="content">The raw content to parse from.</param>
    /// <param name="body">The conversion result, null if couldn't convert.</param>
    /// <typeparam name="TBody">The type of body to convert to.</typeparam>
    /// <returns>A boolean instance whether the conversion was successful or not.</returns>
    public static bool TryParse<TBody>(MemoryStream content, [NotNullWhen(true)] out TBody? body) where TBody : IBody
    {
        try
        {
            body = Parse<TBody>(content);
            return true;
        }
        catch
        {
            body = default;
            return false;
        }
    }
}