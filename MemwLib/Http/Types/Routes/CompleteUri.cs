using JetBrains.Annotations;

namespace MemwLib.Http.Types.Routes;

/// <summary>Complete URI implementation from partial URI class, adds the host, port and protocol.</summary>
[PublicAPI]
public class CompleteUri : PartialUri
{
    /// <summary>The URI protocol to follow, instructs the server/client how to behave.</summary>
    public Protocol Protocol { get; set; }
    
    /// <summary>The domain name, serves as key for the DNS server to resolve an IP.</summary>
    /// <remarks>This property doesn't check for TLD validity.</remarks>
    public string HostName { get; set; }
    
    /// <summary>The username from basic authentication.</summary>
    public string? User { get; set; }
    
    /// <summary>The password from basic authentication.</summary>
    public string? Password { get; set; }
    
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
        if (!uri.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            throw new FormatException("Error while parsing a complete URI, missing protocol.");
        
        string[] splitUri = uri.Split('/');
        
        Protocol = splitUri[0][..^1].Equals("https", StringComparison.OrdinalIgnoreCase)
            ? Protocol.Https
            : Protocol.Http;

        string[] sAHost = splitUri[2].Split('@');

        if (sAHost.Length < 2)
        {
            string[] sHPort = sAHost[0].Split(':');
            
            HostName = sHPort[0];

            if (sHPort.Length > 1)
                Port = ushort.Parse(sHPort[1]);

            if (sHPort.Length > 2)
                throw new FormatException("Invalid Host:Port pair.");
            return;
        }

        {
            string[] sHPort = sAHost[1].Split(':');

            HostName = sHPort[0];

            if (sHPort.Length > 1)
                Port = ushort.Parse(sHPort[1]);

            if (sHPort.Length > 2)
                throw new FormatException("Invalid Host:Port pair.");
        }
        
        {
            string[] sUPassword = sAHost[0].Split(':');

            if (sUPassword.Length != 2)
                throw new FormatException("Invalid user authentication.");

            User = sUPassword[0];
            Password = sUPassword[1];
        }
    }
    
    /// <summary>Constructs the URI contained in the instance as a String.</summary>
    /// <returns>The current instance as a String.</returns>
    public override string ToString()
    {
        string auth = string.Empty;

        if (User != null && Password != null)
            auth += $"{User}:{Password}@";

        string port = string.Empty;

        if (Port is not (443 or 80))
            port += ":" + Port;
        
        return $"{Protocol.ToString().ToLower()}://{auth}{HostName}{port}{base.ToString()}";
    }

    /// <summary>Runs the ToString() method from the right operand.</summary>
    /// <param name="instance">The right operand to get the string from.</param>
    /// <returns>The result of ToString() in the right operand.</returns>
    public static explicit operator string(CompleteUri instance)
        => instance.ToString();
}