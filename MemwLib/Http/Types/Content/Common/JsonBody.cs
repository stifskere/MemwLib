using System.Text;
using JetBrains.Annotations;
using MemwLib.CoreUtils;
using MemwLib.Data.Json;

namespace MemwLib.Http.Types.Content.Common;

/// <summary>JsonBody handles JSON and lets you cast to any type.</summary>
/// <remarks>
/// This uses the MemwLib.Data.Json API, refer to
/// it's documentation for more information about errors and parsing.
/// </remarks>
[PublicAPI]
public class JsonBody : IBody
{
    /// <inheritdoc cref="IBody.ContentType"/>
    public string ContentType => "application/json";

    private readonly string _jsonContent;

    /// <summary>Create a JsonBody from a string.</summary>
    /// <param name="content">The string that contains the JSON payload.</param>
    /// <exception cref="ArgumentException">The payload is not valid JSON.</exception>
    public JsonBody(string content)
    {
        if (!JsonParser.IsValidJson(content))
            throw new ArgumentException("The passed JSON is invalid.", nameof(content));

        _jsonContent = content;
    }

    /// <summary>Create a JsonBody from an object.</summary>
    /// <param name="content">the object to serialize.</param>
    public JsonBody(object content)
    {
        _jsonContent = JsonParser.Serialize(content);
    }

    /// <summary>Deserialize this body to a type.</summary>
    /// <typeparam name="TResult">The type of the object to create.</typeparam>
    /// <returns>An instance of TResult with the content of this body.</returns>
    public TResult? ReadAs<TResult>() 
        => JsonParser.Deserialize<TResult>(_jsonContent);

    /// <inheritdoc cref="IBody.ParseImpl"/>
    public static IBody ParseImpl(MemoryStream content)
        => new JsonBody(content.GetRaw());

    /// <inheritdoc cref="IBody.ToArray"/>
    public byte[] ToArray() 
        => Encoding.ASCII.GetBytes(_jsonContent);
}