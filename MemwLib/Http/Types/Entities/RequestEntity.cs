using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Content;
using MemwLib.Http.Types.Exceptions;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Types.Entities;

/// <summary>BaseEntity implementation for HTTP requests.</summary>
[PublicAPI]
public sealed class RequestEntity : BaseEntity
{
    private RequestMethodType _requestMethod;

    /// <summary>Session parameters passed by middleware.</summary>
    public SessionParameterCollection SessionParameters { get; internal set; } = new();
    
    /// <summary>The request method for this HTTP request entity.</summary>
    /// <remarks>This does not support flags.</remarks>
    /// <exception cref="ArgumentException">Throws when flags were set for this property.</exception>
    [PublicAPI]
    public RequestMethodType RequestMethod
    {
        get => _requestMethod;
        set
        {
            if (value.ToString().Split(',').Length > 1)
                throw new ArgumentException("Request method does not support flags.", nameof(value));

            _requestMethod = value;
        }
    }
    
    /// <summary>The request location as a PartialUri instance.</summary>
    [PublicAPI]
    public PartialUri Path { get; set; }

    /// <summary>The HTTP protocol version for this request.</summary>
    /// <remarks>Due to implementation the http version doesn't modify behavior YET.</remarks>
    [PublicAPI]
    public string HttpVersion { get; }

    /// <summary>
    /// If your route declaration contained RegEx,
    /// you can access RegEx capture groups trough this property.
    /// </summary>
    public HttpRegexGroupCollection CapturedGroups { get; internal set; } = new();
    
    /// <summary>Stream constructor, reads a stream into an instance of RequestEntity.</summary>
    /// <param name="reader">The entity to parse.</param>
    /// <exception cref="ParseException{T}">There was an error while parsing this stream.</exception>
    /// <remarks>The reader must be positioned at the first line of the content.</remarks>
    public RequestEntity(StreamReader reader)
    {
        string[] splitTarget = InitEntity(reader).Split(' ');

        if (splitTarget.Length != 3)
            throw new ParseException<RequestEntity>();
            
        RequestMethod = (RequestMethodType)Enum.Parse(typeof(RequestMethodType), splitTarget[0], true);
        Path = new PartialUri(splitTarget[1]);
        HttpVersion = splitTarget[2];
    }
    
    /// <summary>Parameterized constructor for request entity.</summary>
    /// <param name="method">The method for this entity.</param>
    /// <param name="path">The path for this entity as a PartialUri instance.</param>
    /// <param name="body">The body for this entity.</param>
    public RequestEntity(RequestMethodType method, PartialUri path, IBody? body = null) : this(method, path, "HTTP/1.1", body) {}
    
    /// <inheritdoc cref="RequestEntity(RequestMethodType, PartialUri, IBody?)"/>
    /// <param name="version">the version of the standard this request follows.</param>
    /// <exception cref="FormatException">The HTTP version is invalid.</exception>
    /// <remarks>The version doesn't change the functionality, it's just parsed as string to be sent with the entity.</remarks>
#pragma warning disable CS1573
    public RequestEntity(RequestMethodType method, PartialUri path, string version, IBody? body = null)
#pragma warning restore CS1573
    {
        RequestMethod = method;
        
        Path = path;
        
        if (!HttpVersionRegex().IsMatch(version))
            throw new FormatException("Invalid http version");

        HttpVersion = version;
        
        Body = new BodyConverter(body);
    }
    
    /// <inheritdoc cref="BaseEntity.BuildStart"/>
    protected override string BuildStart() 
        => $"{RequestMethod.ToString().ToUpper()} {(string)Path} {HttpVersion}";
}