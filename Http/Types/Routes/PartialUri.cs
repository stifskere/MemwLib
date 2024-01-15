using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using ParameterCollection = MemwLib.Http.Types.Collections.ParameterCollection;

namespace MemwLib.Http.Types.Routes;

/// <summary>A class that represents a partial URI for request bodies.</summary>
/// <example>/route?key=value#fragment</example>
public partial class PartialUri
{
    /// <summary>The path where the server is supposed to look for data.</summary>
    [PublicAPI]
    public string Route { get; protected set; }
    
    /// <summary>A collection of the URI parameters whose are after the (?) question mark.</summary>
    [PublicAPI]
    public ParameterCollection Parameters { get; protected set; }
    
    /// <summary>The fragment which is defined after the (#) hashtag or NULL if not none.</summary>
    /// <remarks>Not supported in rfc9112 standard, but left for media fragment resolution in edge cases.</remarks>
    [PublicAPI]
    public string? Fragment { get; protected set; }

    /// <summary>
    /// Default constructor for partial URI class,
    /// constructs the URI from the formatted string following the RFC1808 standard.
    /// </summary>
    /// <param name="uri">The formatted URI.</param>
    /// <exception cref="FormatException">The passed URI is not in a valid format.</exception>
    public PartialUri(string uri)
    {
        if (uri.EndsWith('#') || uri.EndsWith('?'))
            uri = uri[..^1];
        
        Match matchedUri = PartialUriRegex().Match(uri);

        if (!matchedUri.Success)
            throw new FormatException("Partial URI is not in a valid format.");
        
        Route = matchedUri.Groups["path"].Value;
        
        Parameters = matchedUri.Groups.ContainsKey("params") 
            ? new ParameterCollection(matchedUri.Groups["params"].Value) 
            : new ParameterCollection();
        
        Fragment = matchedUri.Groups.ContainsKey("fragment")
            ? matchedUri.Groups["fragment"].Value
            : null;
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
        => $"{Route}{(Parameters.Length != 0 ? $"?{(string)Parameters}" : "")}{(string.IsNullOrEmpty(Fragment) ? "" : $"#{Fragment}")}";
    
    /// <summary>Runs the ToString() method from the right operand.</summary>
    /// <param name="instance">The right operand to get the string from.</param>
    /// <returns>The result of ToString() in the right operand.</returns>
    public static explicit operator string(PartialUri instance)
        => instance.ToString();
    
    [GeneratedRegex(@"(?:https?:\/\/[^/]*)?(?'path'\/[^?#{}|\\^~\[\]\/][^?#{}|\^~[\]]*)?(?'params'\?[^#]*)?(?'fragment'#.*)?$", RegexOptions.Singleline)]
    private static partial Regex PartialUriRegex();
}