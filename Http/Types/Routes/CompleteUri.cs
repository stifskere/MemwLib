using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Routes;

public partial class CompleteUri : PartialUri
{
    [PublicAPI]
    public Protocol Protocol { get; set; }
    
    [PublicAPI]
    public string Name { get; set; }
    
    [PublicAPI]
    public ushort Port { get; set; }
    
    public CompleteUri(string content) : base(content)
    {
        Match matchedUri = UriRegex().Match(content);

        if (!matchedUri.Success)
            throw new ArgumentException("Complete URI is not in a valid format.", nameof(content));
        
        Protocol = matchedUri.Groups.ContainsKey("protocol") 
            ? Enum.Parse<Protocol>(matchedUri.Groups["protocol"].Value, true)
            : Protocol.Http;

        string[] name = matchedUri.Groups["name"].Value.Split(':');

        Name = name[0];
        
        Port = name.Length == 2 ? ushort.Parse(name[1]) : (ushort)Protocol;
            
    }

    public override string ToString()
        => $"{Protocol.ToString().ToLower()}://{Name}{base.ToString()}";

    public static explicit operator string(CompleteUri instance)
        => instance.ToString();
    
    
    [GeneratedRegex(@"^(?:(?'protocol'https?)\:\/\/)?(?'name'[a-z\-0-9.]+(?::\d{1,5})?)\/?", RegexOptions.Singleline)]
    private static partial Regex UriRegex();
}