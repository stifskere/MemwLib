using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Types.Entities;

/// <summary>BaseEntity implementation for HTTP requests.</summary>
[PublicAPI]
public sealed partial class RequestEntity : BaseEntity
{
    private RequestMethodType _requestType;

    /// <summary>The request method for this HTTP request entity.</summary>
    /// <remarks>This does not support flags.</remarks>
    /// <exception cref="ArgumentException">Throws when flags were set for this property.</exception>
    [PublicAPI]
    public RequestMethodType RequestType
    {
        get => _requestType;
        set
        {
            if (value.ToString().Split(',').Length > 1)
                throw new ArgumentException("Request method does not support flags.", nameof(value));

            _requestType = value;
        }
    }
    
    /// <summary>The request location as a PartialUri instance.</summary>
    [PublicAPI]
    public PartialUri Path { get; set; }
    
    /// <summary>The HTTP protocol version for this request.</summary>
    [PublicAPI]
    public string HttpVersion { get; }
    
    /// <summary>String constructor, parses an ASCII string into an instance of RequestEntity.</summary>
    /// <param name="stringEntity">The entity to parse.</param>
    /// <exception cref="ArgumentException">The string entity is null or empty.</exception>
    /// <exception cref="FormatException">The request was not well formatted.</exception>
    public RequestEntity(string stringEntity)
    {
        if (string.IsNullOrEmpty(stringEntity))
            throw new ArgumentException("entity cannot be null or empty", nameof(stringEntity));
        
        string[] lines = stringEntity.Split("\r\n");
        
        {
            Match startLine = StartLineRegex().Match(lines[0]);

            if (!startLine.Success)
                throw new FormatException("Malformed request start");
            
            RequestType = Enum.Parse<RequestMethodType>(startLine.Groups["method"].Value, true);
            Path = new PartialUri(startLine.Groups["path"].Value);
            HttpVersion = startLine.Groups["version"].Value;
        }
        
        if (lines.Length < 2)
        {
            Headers = new HeaderCollection();
            Body = string.Empty;
            return;
        }


        int separatorIndex = Array.IndexOf(lines, string.Empty);

        Headers = new HeaderCollection(string.Join("\r\n", lines[1..separatorIndex]));
        
        string provisionalBody = string.Join("\r\n", lines[(separatorIndex + 1)..]);

        if (Headers.Contains("Content-Length") && int.TryParse(Headers["Content-Length"], out int contentLength))
        {
            if (provisionalBody.Length < contentLength)
                for (int i = 0, fixBodyLength = provisionalBody.Length; i < contentLength - fixBodyLength; i++)
                    provisionalBody += ' ';
            else
                provisionalBody = provisionalBody[..contentLength];
        }

        Body = provisionalBody;
    }
    
    /// <summary>Parameterized constructor for request entity.</summary>
    /// <param name="type">The method for this entity.</param>
    /// <param name="path">The path for this entity as a PartialUri instance.</param>
    /// <param name="body">The body for this entity.</param>
    public RequestEntity(RequestMethodType type, PartialUri path, string? body = null) : this(type, path, "HTTP/1.1", body) {}
    
    /// <inheritdoc cref="RequestEntity(RequestMethodType, PartialUri, string?)"/>
    /// <param name="version">the version of the standard this request follows.</param>
    /// <exception cref="FormatException">The HTTP version is invalid.</exception>
    /// <remarks>The version doesn't change the functionality, it's just parsed as string to be sent with the entity.</remarks>
#pragma warning disable CS1573
    public RequestEntity(RequestMethodType type, PartialUri path, string version, string? body = null)
#pragma warning restore CS1573
    {
        RequestType = type;
        
        Path = path;
        
        if (!HttpVersionRegex().IsMatch(version))
            throw new FormatException("Invalid http version");

        HttpVersion = version;
        
        Body = body ?? string.Empty;
    }

    /// <inheritdoc cref="BaseEntity.BuildStart"/>
    protected override string BuildStart() 
        => $"{RequestType.ToString().ToUpper()} {(string)Path} {HttpVersion}";

    [GeneratedRegex(@"(?'method'OPTIONS|GET|HEAD|POST|PATCH|PUT|DELETE|TRACE|CONNECT) (?'path'\/[^ ]*) (?'version'HTTP\/\d+\.\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StartLineRegex();

    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase)]
    private static partial Regex HttpVersionRegex();
}