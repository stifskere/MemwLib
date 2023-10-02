using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Routes;

public partial class PartialUri
{
    [PublicAPI]
    public string Route { get; protected set; }
    
    [PublicAPI]
    public ParameterCollection Parameters { get; protected set; }
    
    [PublicAPI]
    public string? Fragment { get; protected set; }

    public PartialUri(string uri)
    {
        if (uri.EndsWith('#') || uri.EndsWith('?'))
            uri = uri[..^1];
        
        Match matchedUri = PartialUriRegex().Match(uri);

        if (!matchedUri.Success)
            throw new ArgumentException("Partial URI is not in a valid format.", nameof(uri));
        
        Route = matchedUri.Groups["path"].Value;
        
        Parameters = matchedUri.Groups.ContainsKey("params") 
            ? new ParameterCollection(matchedUri.Groups["params"].Value) 
            : new ParameterCollection();

        Fragment = matchedUri.Groups.ContainsKey("fragment")
            ? matchedUri.Groups["fragment"].Value
            : null;
    }

    public override string ToString()
        => $"{Route}{(Parameters.Length != 0 ? $"?{(string)Parameters}" : "")}{(string.IsNullOrEmpty(Fragment) ? "" : $"#{Fragment}")}";
    
    public static explicit operator string(PartialUri instance)
        => instance.ToString();
    
    [GeneratedRegex(@"^https?://[^/]*(?'path'\/[^?#{}|\\^~\[\]`]*)?(?'params'\?[^#]*)?(?'fragment'#.*)?$", RegexOptions.Singleline)]
    private static partial Regex PartialUriRegex();
}