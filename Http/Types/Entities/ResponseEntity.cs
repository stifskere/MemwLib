using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Entities;

/// <summary>BaseEntity implementation for HTTP responses.</summary>
public sealed partial class ResponseEntity : BaseEntity
{
    /// <summary>The HTTP protocol version for this request.</summary>
    [PublicAPI]
    public string HttpVersion { get; } = "HTTP/1.1";
    
    /// <summary>The response code for this request.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Status codes on MDN.</see>
    [PublicAPI]
    public short ResponseCode { get; set; }
    
    /// <summary>The corresponding hint for the response code.</summary>
    /// <exception cref="ConstraintException">The response code is not valid.</exception>
    [PublicAPI]
    public string Hint => GetResponseCodeHint();

    /// <summary>Returns true if the response code is 100-399 otherwise false.</summary>
    [PublicAPI] 
    public bool IsSuccessfulResponse => ResponseCode < 400;
    
    /// <summary>Parser constructor for </summary>
    /// <param name="stringEntity"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public ResponseEntity(string stringEntity)
    {
        if (string.IsNullOrEmpty(stringEntity))
            throw new ArgumentException("entity cannot be null or empty", nameof(stringEntity));
        
        string[] lines = stringEntity.Split("\r\n");

        {
            Match startLine = StartLineRegex().Match(lines[0]);

            if (!startLine.Success)
                throw new Exception("Malformed request start");

            HttpVersion = startLine.Groups[1].Value;
            ResponseCode = short.Parse(startLine.Groups[2].Value);
        }

        int separatorIndex = Array.IndexOf(lines, string.Empty);
        
        Headers = new HeaderCollection(string.Join("\r\n", lines.Skip(0).Take(1..(separatorIndex == -1 ? lines.Length - 1 : separatorIndex))));
        
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
    
    protected override string BuildStart()
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
            _ => throw new ConstraintException("Unknown response code")
        };
    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex HttpVersionRegex();
    
    [GeneratedRegex(@"(HTTP\/\d+\.\d+) (\d+) .+", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex StartLineRegex();
}