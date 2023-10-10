using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Types.Builders;

/// <summary>Represents a client made HTTP request.</summary>
public class RequestBuilder
{
    private RequestMethodType _type;
    private CompleteUri _uri;
    private string? _body;
    private readonly HeaderCollection _headers = new();
    
    /// <summary>Request builder constructor to define the required request fields.</summary>
    /// <param name="uri">The URI field, constructs a URI internally.</param>
    /// <param name="type">The request type for the server to interpret, DOES NOT SUPPORT FLAGS.</param>
    /// <param name="body">Optional body parameter, sets the request body.</param>
    /// <param name="headers">Optional headers parameter, adds headers from a (String, String) tuple array.</param>
    public RequestBuilder(string uri, RequestMethodType type = RequestMethodType.Get, string? body = null, params (string key, string value)[] headers)
    {
        _type = type;
        _uri = new CompleteUri(uri);
        _body = body;
        
        foreach ((string key, string value) in headers)
            _headers[key] = value;
        
    }
    
    /// <summary>Replaces the set request method by the constructor.</summary>
    /// <param name="type">The request method type to set.</param>
    /// <returns>The same instance.</returns>
    [PublicAPI]
    public RequestBuilder SetRequestMethod(RequestMethodType type)
    {
        _type = type;
        return this;
    }

    /// <summary>Replaces the set URI by the constructor.</summary>
    /// <param name="uri">The URI to set.</param>
    /// <returns>The same instance.</returns>
    [PublicAPI]
    public RequestBuilder SetUri(string uri)
    {
        _uri = new CompleteUri(uri);
        return this;
    }
    
    /// <summary>Adds a header to the request.</summary>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    /// <returns>The same instance.</returns>
    [PublicAPI]
    public RequestBuilder AddHeader(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    /// <summary>Replaces the request body.</summary>
    /// <param name="body">The body to set.</param>
    /// <returns>The same instance.</returns>
    [PublicAPI]
    public RequestBuilder SetBody(string body)
    {
        _body = body;
        return this;
    }
    
    internal (CompleteUri uri, RequestEntity entity) Build()
        => (_uri, new RequestEntity(_type, _uri, _body));
}