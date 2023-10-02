using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Types.Entities;


[PublicAPI]
public sealed partial class RequestEntity : BaseEntity
{
    [PublicAPI]
    public RequestMethodType RequestType { get; set; }
    
    [PublicAPI]
    public PartialUri Path { get; set; }
    
    [PublicAPI]
    public string HttpVersion { get; }
    
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
    
    public RequestEntity(RequestMethodType type, PartialUri path, string? body = null) : this(type, path, "HTTP/1.1", body) {}
    public RequestEntity(RequestMethodType type, PartialUri path, string version, string? body = null)
    {
        RequestType = type;
        
        Path = path;

        if (!HttpVersionRegex().IsMatch(version))
            throw new ArgumentException("Invalid http version", nameof(version));

        HttpVersion = version;
        
        Body = body ?? string.Empty;
    }

    public override string BuildStart() 
        => $"{RequestType.ToString().ToUpper()} {(string)Path} {HttpVersion}";

    [GeneratedRegex(@"(?'method'OPTIONS|GET|HEAD|POST|PATCH|PUT|DELETE|TRACE|CONNECT) (?'path'\/[^ ]*) (?'version'HTTP\/\d+\.\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StartLineRegex();

    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase)]
    private static partial Regex HttpVersionRegex();
}