using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Content;
using MemwLib.Http.Types.Exceptions;
using HeaderCollection = MemwLib.Http.Types.Collections.HeaderCollection;

namespace MemwLib.Http.Types.Entities;

/// <summary>BaseEntity implementation for HTTP responses.</summary>
[PublicAPI]
public sealed partial class ResponseEntity : BaseEntity, IResponsible
{
    /// <summary>The HTTP protocol version for this request.</summary>
    /// <remarks>Due to implementation the http version doesn't modify behavior YET.</remarks>
    public string HttpVersion { get; } = "HTTP/1.1";
    
    /// <summary>The response code for this request.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Status codes on MDN.</see>
    public ResponseCodes ResponseCode { get; set; }

    /// <summary>Returns true if the response code is 100-399 otherwise false.</summary>
    public bool IsSuccessfulResponse => (int)ResponseCode < 400;
    
    /// <summary>Stream constructor, reads a stream into an instance of ResponseEntity.</summary>
    /// <param name="reader">The entity to parse.</param>
    /// <exception cref="ParseException{T}">There was an error while parsing this stream.</exception>
    /// <remarks>The reader must be positioned at the first line of the content.</remarks>
    // TODO: requires abstraction, implement timeout
    public ResponseEntity(StreamReader reader)
    {
        string? target;
        
        while ((target = reader.ReadLine()) == null) { }
        
        {
            string[] splitTarget = target.Split(' ');

            if (splitTarget.Length != 3)
                throw new ParseException<ResponseEntity>();
            
            HttpVersion = splitTarget[0];
            ResponseCode = (ResponseCodes)Enum.Parse(typeof(ResponseCodes), splitTarget[1], true);
        }

        Headers = new HeaderCollection();
        
        while (!string.IsNullOrEmpty(target = reader.ReadLine()))
        {
            string[] splitTarget = target.Split(": ");

            if (splitTarget.Length != 2)
                throw new ParseException<RequestEntity>();

            Headers[splitTarget[0]] = splitTarget[1];
        }
        
        if (!Headers.Contains("Content-Length"))
        {
            Body = BodyConverter.Empty;
            return;
        }

        if (!int.TryParse(Headers["Content-Length"], out int bodyLength))
            throw new ParseException<RequestEntity>();

        byte[] body = new byte[bodyLength];

        int index = 0;
        while (bodyLength-- > 0)
        {
            int read = reader.Read();

            if (read == -1)
            {
                Headers["Content-Length"] = index.ToString();
                break;
            }

            body[index++] = (byte)read;
        }

        Body = new BodyConverter(Encoding.ASCII.GetString(body));
    }

    /// <summary>Parameterized constructor for ResponseEntity.</summary>
    /// <param name="responseCode">The response code for this entity.</param>
    /// <param name="body">The request body for this entity.</param>
    public ResponseEntity(ResponseCodes responseCode, IBody? body = null) : this(responseCode, null, body) {}
    
    /// <inheritdoc cref="ResponseEntity(ResponseCodes, IBody?)"/>
    /// <param name="version">the version of the standard this request follows.</param>
    /// <exception cref="FormatException">The HTTP version is invalid.</exception>
    /// <remarks>The version doesn't change the functionality, it's just parsed as string to be sent with the entity.</remarks>
#pragma warning disable CS1573
    public ResponseEntity(ResponseCodes responseCode, string? version, IBody? body)
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
        => $"{HttpVersion} {ResponseCode.GetCode()} {ResponseCode.GetName()}";
    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex HttpVersionRegex();
    
    [GeneratedRegex(@"(HTTP\/\d+\.\d+) (\d+) .+", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex StartLineRegex();
}