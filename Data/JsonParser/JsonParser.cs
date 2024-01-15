#if DEBUG

using JetBrains.Annotations;

namespace MemwLib.Data.JsonParser;

/// <summary>Definition for JSON parsing static methods.</summary>
[PublicAPI]
public static class JsonParser
{
    /// <summary>Converts a valid JSON string into the specified type.</summary>
    /// <param name="payload">The json payload to convert.</param>
    /// <typeparam name="TResult">The type that should result from the conversion.</typeparam>
    /// <returns>An instance of TResult filled based on the payload parameter.</returns>
    /// <exception cref="InvalidJsonException">Thrown when the json payload contains invalid JSON.</exception>
    public static TResult Deserialize<TResult>(string payload) where TResult : new()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>Converts a class instance to a JSON string.</summary>
    /// <param name="payload">The class instance to convert.</param>
    /// <typeparam name="TPayload">The type of the class instance.</typeparam>
    /// <returns>A string instance representing the TPayload instance as JSON.</returns>
    public static string Serialize<TPayload>(TPayload payload)
    {
        throw new NotImplementedException();
    }
}

#endif