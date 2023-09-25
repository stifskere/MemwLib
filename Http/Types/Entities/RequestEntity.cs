using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Entities;

[PublicAPI]
public sealed partial class RequestEntity : BaseEntity
{
    [PublicAPI]
    public RequestMethodType RequestType { get; set; }
    
    [PublicAPI]
    public string Path { get; set; }
    
    [PublicAPI]
    public ParameterCollection Parameters { get; set; } = new();
    
    [PublicAPI]
    public string Fragment { get; set; } = string.Empty;
    
    [PublicAPI]
    public string HttpVersion { get; } = "HTTP/1.1";
    
    public RequestEntity(string stringEntity)
    {
        if (string.IsNullOrEmpty(stringEntity))
            throw new ArgumentException("entity cannot be null or empty", nameof(stringEntity));
        
        string[] lines = stringEntity.Split("\r\n");
        
        {
            Match startLine = StartLineRegex().Match(lines[0]);

            if (!startLine.Success)
                throw new Exception("Malformed request start");
            
            RequestType = Enum.Parse<RequestMethodType>(startLine.Groups["method"].Value, true);
            Path = startLine.Groups["path"].Value;
            Parameters = new ParameterCollection(startLine.Groups["parameters"].Value);
            Fragment = startLine.Groups["fragment"].Value;
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
    
    public RequestEntity(RequestMethodType type, string path, string? body = null) : this(type, path, null, body) {}
    public RequestEntity(RequestMethodType type, string path, string? version = null, string? body = null)
    {
        RequestType = type;

        if (!path.StartsWith('/'))
            throw new ArgumentException("Path should start with /", nameof(path));

        Path = path;

        if (version is not null)
        {
            if (!HttpVersionRegex().IsMatch(version))
                throw new ArgumentException("Invalid http version", nameof(version));

            HttpVersion = version;
        }
        
        Body = body ?? string.Empty;
    }

    public override string BuildStart()
        => $"{RequestType} {Path}?{(string)Parameters} {HttpVersion}";

    [GeneratedRegex(@"(?'method'OPTIONS|GET|HEAD|POST|PATCH|PUT|DELETE|TRACE|CONNECT) (?'path'\/[^?#]+)(?:\?(?'parameters'[^#]*))?(?:#(?'fragment'.*))? (?'version'HTTP\/\d+\.\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StartLineRegex();

    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase)]
    private static partial Regex HttpVersionRegex();
}