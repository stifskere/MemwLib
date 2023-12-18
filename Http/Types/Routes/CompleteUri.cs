using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Routes;

/// <summary>Complete URI implementation from partial URI class, adds the host, port and protocol.</summary>
public partial class CompleteUri : PartialUri
{
    /// <summary>The URI protocol to follow, instructs the server/client how to behave.</summary>
    [PublicAPI]
    public Protocol Protocol { get; set; }
    
    /// <summary>The domain name, serves as key for the DNS server to resolve an IP.</summary>
    /// <remarks>This property doesn't check for TLD validity.</remarks>
    [PublicAPI]
    public string HostName { get; set; }
    
    /// <summary>
    /// The port number where to establish the connection,
    /// if unset will use protocol default ports as for common TCP ports.
    /// </summary>
    [PublicAPI]
    public ushort Port
    {
        get => _port ?? (ushort)Protocol;
        set => _port = value;
    }
    
    private ushort? _port;
    
    /// <summary>
    /// Default constructor for Complete URI, constructs the URI
    /// from the formatted string following the RFC1808 standard.
    /// </summary>
    /// <param name="uri">The formatted URI.</param>
    /// <exception cref="FormatException">The passed URI is not in a valid format.</exception>
    public CompleteUri(string uri) : base(uri)
    {
        Match matchedUri = UriRegex().Match(uri);

        if (!matchedUri.Success)
            throw new FormatException("Complete URI is not in a valid format.");
        
        Protocol = matchedUri.Groups.ContainsKey("protocol") 
            ? Enum.Parse<Protocol>(matchedUri.Groups["protocol"].Value, true)
            : Protocol.Http;

        string[] name = matchedUri.Groups["name"].Value.Split(':');

        HostName = name[0];
        
        _port = name.Length == 2 ? ushort.Parse(name[1]) : null;
            
    }
    
    /// <summary>Constructs the URI contained in the instance as a String.</summary>
    /// <returns>The current instance as a String.</returns>
    public override string ToString()
        => $"{Protocol.ToString().ToLower()}://{HostName}{base.ToString()}";

    /// <summary>Runs the ToString() method from the right operand.</summary>
    /// <param name="instance">The right operand to get the string from.</param>
    /// <returns>The result of ToString() in the right operand.</returns>
    public static explicit operator string(CompleteUri instance)
        => instance.ToString();
    
    
    [GeneratedRegex(@"^(?:(?'protocol'https?)\:\/\/)?(?'name'[a-z\-0-9.]+(?::\d{1,5})?)\/?", RegexOptions.Singleline)]
    private static partial Regex UriRegex();
}