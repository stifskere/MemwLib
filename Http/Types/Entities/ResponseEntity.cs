using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Entities;

public sealed partial class ResponseEntity : BaseEntity
{
    [PublicAPI]
    public string HttpVersion { get; } = "HTTP/1.1";
    
    [PublicAPI]
    public short ResponseCode { get; set; }
    
    [PublicAPI]
    public string Hint => GetResponseCodeHint();

    [PublicAPI] 
    public bool IsSuccessfulResponse => ResponseCode < 400;
    
    public ResponseEntity(string stringEntity)
    {
        if (string.IsNullOrEmpty(stringEntity))
            throw new ArgumentException("entity cannot be null or empty", nameof(stringEntity));
        
        string[] lines = stringEntity.Split("\r\n");

        {
            Match startLine = StartLineRegex().Match(lines[0]);

            if (!startLine.Success)
                throw new Exception("Malformed request start");

            HttpVersion = startLine.Groups[0].Value;
            ResponseCode = short.Parse(startLine.Groups[1].Value);
        }

        int indexOfHbSeparator = Array.IndexOf(lines, " ");
        Headers = new HeaderCollection(string.Join("\r\n", lines.Skip(0).Take(1..indexOfHbSeparator)));
        Body = lines.Length >= indexOfHbSeparator ? lines[indexOfHbSeparator + 1] : string.Empty;
    }
    
    public ResponseEntity(short responseCode) : this(responseCode, null) {}
    public ResponseEntity(short responseCode, string? body = null) : this(responseCode, null, body) {}
    public ResponseEntity(short responseCode, string? version = null, string? body = null)
    {
        ResponseCode = responseCode;

        if (version is not null)
        {
            if (!HttpVersionRegex().IsMatch(version))
                throw new ArgumentException("Invalid http version", nameof(version));

            HttpVersion = version;
        }

        Body = body ?? string.Empty;
    }
    
    public override string BuildStart()
        => $"{HttpVersion} {ResponseCode} {Hint}";

    private string GetResponseCodeHint()
        => ResponseCode switch
        {
            100 => "Continue",
            101 => "Switching Protocols",
            200 => "OK",
            201 => "Created",
            202 => "Accepted",
            203 => "Non-Authoritative Information",
            204 => "No Content",
            205 => "Reset Content",
            206 => "Partial Content",
            300 => "Multiple Choices",
            301 => "Moved Permanently",
            302 => "Found",
            303 => "See Other",
            304 => "Not Modified",
            305 => "Use Proxy",
            307 => "Temporary Redirect",
            400 => "Bad Request",
            401 => "Unauthorized",
            402 => "Payment Required",
            403 => "Forbidden",
            404 => "Not Found",
            405 => "Method Not Allowed",
            406 => "Not Acceptable",
            407 => "Proxy Authentication Required",
            408 => "Request Timeout",
            409 => "Conflict",
            410 => "Gone",
            411 => "Length Required",
            412 => "Precondition Failed",
            413 => "Request Entity Too Large",
            414 => "Request-URI Too Long",
            415 => "Unsupported Media Type",
            416 => "Requested Range Not Satisfiable",
            417 => "Expectation Failed",
            500 => "Internal Server Error",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            504 => "Gateway Timeout",
            505 => "HTTP Version Not Supported",
            _ => throw new Exception("Unknown response code"),
        };
    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex HttpVersionRegex();
    
    [GeneratedRegex(@"(HTTP\/\d+\.\d+) (\d+) .+", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex StartLineRegex();
}