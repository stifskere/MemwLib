using System.Data;
using System.Text;
using JetBrains.Annotations;
using MemwLib.CoreUtils;
using MemwLib.CoreUtils.Collections;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Types.Content.Implementations;

/// <summary>Reads and parses a post x-www-form-urlencoded body.</summary>
/// <example>name=john&amp;age=20</example>
[PublicAPI]
public class UrlEncodedBody : BaseIsolatedCollection<string, string>, IBody
{
    /// <inheritdoc cref="IBody.ContentType"/>
    public string ContentType => "application/x-www-form-urlencoded";

    /// <summary>Initializes an instance of x-www-form-urlencoded body.</summary>
    /// <param name="pairs">The pairs the body will contain.</param>
    public UrlEncodedBody(params KeyValuePair<string, string>[] pairs)
    {
        foreach ((string key, string value) in pairs)
            base.Set(key, value);
    }
    
    /// <inheritdoc cref="IBody.ParseImpl"/>
    public static IBody ParseImpl(MemoryStream streamContent)
    {
        string content = streamContent.GetRaw();
        
        if (string.IsNullOrEmpty(content))
            return new UrlEncodedBody();
        
        string[] split = content.Split('&');
        
        KeyValuePair<string, string>[] pairs = new KeyValuePair<string, string>[split.Length];
        
        for (int i = 0; i < split.Length; i++)
        {
            string[] kvp = split[i].Split('=');
            
            if (kvp.Length != 2)
                throw new ConstraintException("a URI query parameter must be a valid key value pair with the format key=value");
            
            pairs[i] = new KeyValuePair<string, string>(UriHelpers.DecodeUriComponent(kvp[0]), UriHelpers.DecodeUriComponent(kvp[1]));
        }

        return new UrlEncodedBody(pairs);
    }

    /// <inheritdoc cref="IBody.ToArray"/>
    public byte[] ToArray()
    {
        string result = string.Empty;

        foreach ((string key, string value) in this)
            result += $"{UriHelpers.EncodeUriComponent(key)}={UriHelpers.EncodeUriComponent(value)}&";

        return Encoding.ASCII.GetBytes(result[..^1]);
    }
}