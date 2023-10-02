using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Routes;

public partial class Uri : PartialUri
{
    [PublicAPI]
    public Protocol Protocol { get; set; }
    
    [PublicAPI]
    public string Name { get; set; }
    
    public Uri(string content) : base(content)
    {
        Match matchedUri = UriRegex().Match(content);

        Protocol = Enum.Parse<Protocol>(matchedUri.Groups["protocol"].Value, true);
        Name = matchedUri.Groups["name"].Value;
    }

    public override string ToString()
        => $"{Protocol.ToString().ToLower()}://{Name}{base.ToString()}";

    public static explicit operator string(Uri instance)
        => instance.ToString();
    
    [GeneratedRegex(@"^(?'protocol'https?)\:\/\/(?'name'[^/]+)\/?", RegexOptions.Singleline)]
    private static partial Regex UriRegex();
}