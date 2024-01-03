using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Content;
using HeaderCollection = MemwLib.Http.Types.Collections.HeaderCollection;

namespace MemwLib.Http.Types.Entities;

/// <summary>BaseEntity implementation for HTTP responses.</summary>
public sealed partial class ResponseEntity : BaseEntity, IResponsible
{
    /// <summary>The HTTP protocol version for this request.</summary>
    /// <remarks>Due to implementation the http version doesn't modify behavior YET.</remarks>
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
    
    /// <summary>String constructor, parses an ASCII string into an instance of ResponseEntity</summary>
    /// <param name="stringEntity">The entity to parse.</param>
    /// <exception cref="ArgumentException">The entity is null or empty.</exception>
    /// <exception cref="FormatException">The request was not well formatted.</exception>
    public ResponseEntity(string stringEntity)
    {
        if (string.IsNullOrEmpty(stringEntity))
            throw new ArgumentException("entity cannot be null or empty", nameof(stringEntity));
        
        string[] lines = stringEntity.Split("\r\n");

        {
            Match startLine = StartLineRegex().Match(lines[0]);

            if (!startLine.Success)
                throw new FormatException("Malformed request start");

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

        Body = new BodyConverter(provisionalBody);
    }

    /// <summary>Parameterized constructor for ResponseEntity.</summary>
    /// <param name="responseCode">The response code for this entity.</param>
    /// <param name="body">The request body for this entity.</param>
    public ResponseEntity(short responseCode, IBody? body = null) : this(responseCode, null, body) {}
    
    /// <inheritdoc cref="ResponseEntity(short, IBody?)"/>
    /// <param name="version">the version of the standard this request follows.</param>
    /// <exception cref="FormatException">The HTTP version is invalid.</exception>
    /// <remarks>The version doesn't change the functionality, it's just parsed as string to be sent with the entity.</remarks>
#pragma warning disable CS1573
    public ResponseEntity(short responseCode, string? version, IBody? body)
#pragma warning restore CS1573
    {
        ResponseCode = responseCode;

        if (version is not null)
        {
            if (!HttpVersionRegex().IsMatch(version))
                throw new ArgumentException("Invalid http version", nameof(version));

            HttpVersion = version;
        }

        Body = new BodyConverter(body);
    }
    
    /// <inheritdoc cref="BaseEntity.BuildStart"/>
    protected override string BuildStart()
        => $"{HttpVersion} {ResponseCode} {Hint}";

    // NOTE: THIS IS ABSTRACTED AS METHOD FOR CODE NAVIGATION.
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