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

    private string HttpVersion { get; } = "HTTP/1.1";
    
    public RequestEntity(string stringEntity)
    {
        if (string.IsNullOrEmpty(stringEntity))
            throw new ArgumentException("entity cannot be null or empty", nameof(stringEntity));
        
        string[] lines = stringEntity.Split("\r\n");

        {
            Match startLine = StartLineRegex().Match(lines[0]);

            if (!startLine.Success)
                throw new Exception("Malformed request start");
            
            RequestType = Enum.Parse<RequestMethodType>(startLine.Groups[1].Value, true);
            Path = startLine.Groups[2].Value;
            Parameters = new ParameterCollection(startLine.Groups[3].Value);
            HttpVersion = startLine.Groups[4].Value;
        }

        int indexOfHbSeparator = Array.IndexOf(lines, " ");
        Headers = new HeaderCollection(indexOfHbSeparator != -1 ? string.Join("\r\n", lines.Take(1..indexOfHbSeparator)) : string.Empty);
        Body = lines.Length >= indexOfHbSeparator ? lines[indexOfHbSeparator + 1] : string.Empty;
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

    [GeneratedRegex(@"(OPTIONS|GET|HEAD|POST|PATCH|PUT|DELETE|TRACE|CONNECT) (\/[^?]+)(?:\?(.*))? (HTTP\/\d+\.\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StartLineRegex();
    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase)]
    private static partial Regex HttpVersionRegex();
}