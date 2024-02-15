using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Routes;

/// <summary>A class that represents a partial URI for request bodies.</summary>
/// <example>/route?key=value#fragment</example>
[PublicAPI]
public class PartialUri
{
    /// <summary>The path where the server is supposed to look for data.</summary>
    public string Route { get; set; }
    
    /// <summary>A collection of the URI parameters whose are after the (?) question mark.</summary>
    public ParameterCollection Parameters { get; protected set; }
    
    /// <summary>The fragment which is defined after the (#) hashtag or NULL if not none.</summary>
    /// <remarks>
    /// Not supported in rfc9112 standard, but left for media fragment resolution in edge cases.<para/>
    /// This won't be sent in the HTTP client.
    /// </remarks>
    public string? Fragment { get; set; }

    /// <summary>
    /// Default constructor for partial URI class,
    /// constructs the URI from the formatted string following the RFC1808 standard.
    /// </summary>
    /// <param name="uri">The formatted URI.</param>
    /// <exception cref="FormatException">The passed URI is not in a valid format.</exception>
    public PartialUri(string uri)
    {
        if (uri.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            string[] sFirst = uri.Split('/');

            if (sFirst.Length < 4)
                throw new FormatException("the URI is empty.");
            
            uri = "/" + string.Join('/', sFirst[3..]);
        }

        string[] sRParams = uri.Split('?');
        
        Route = string.Join('/', sRParams[0].Split('/').Select(UriHelpers.DecodeUriComponent));
        Parameters = new ParameterCollection();
        
        if (sRParams.Length < 2)
            return;

        foreach (string pair in sRParams[1].Split('&'))
        {
            string[] splitPair = pair.Split('=');

            if (splitPair.Length != 2)
                throw new FormatException("Invalid URI query parameter found.");
            
            Parameters.Set(
                UriHelpers.DecodeUriComponent(splitPair[0]), 
                UriHelpers.DecodeUriComponent(splitPair[1])
            );
        }

        string[] sPFragment = sRParams[1].Split('#');

        if (sPFragment.Length != 1)
            Fragment = UriHelpers.DecodeUriComponent(sPFragment[1]);
    }

    /// <summary>Constructor from CompleteURI to avoid polymorphism issues.</summary>
    /// <param name="uri">The URI to cast from.</param>
    public PartialUri(CompleteUri uri)
    {
        Route = uri.Route;
        Parameters = uri.Parameters;
        Fragment = uri.Fragment;
    }
    
    /// <summary>Constructs the URI contained in the instance as a String.</summary>
    /// <returns>The current instance as a String.</returns>
    public override string ToString()
    {
        string result 
            = string.Join('/', Route.Split('/').Select(UriHelpers.EncodeUriComponent));

        if (Parameters.Length > 0)
            result += "?" + (string)Parameters;

        if (!string.IsNullOrEmpty(Fragment))
            result += "#" + UriHelpers.EncodeUriComponent(Fragment);
        
        return result;
    }
    
    /// <summary>Runs the ToString() method from the right operand.</summary>
    /// <param name="instance">The right operand to get the string from.</param>
    /// <returns>The result of ToString() in the right operand.</returns>
    public static explicit operator string(PartialUri instance)
        => instance.ToString();
}