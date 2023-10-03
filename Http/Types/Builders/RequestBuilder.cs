using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Types.Builders;

public class RequestBuilder
{
    private RequestMethodType _type;
    private CompleteUri _uri;
    private string? _body;
    private readonly HeaderCollection _headers = new();
    
    public RequestBuilder(string uri, RequestMethodType type = RequestMethodType.Get, string? body = null, (string key, string value)[]? headers = null)
        : this(new CompleteUri(uri), type, body, headers) {}
    public RequestBuilder(CompleteUri uri, RequestMethodType type = RequestMethodType.Get, string? body = null, (string key, string value)[]? headers = null)
    {
        _type = type;
        _uri = uri;
        _body = body;

        if (headers is null)
            return;
        
        foreach ((string key, string value) in headers)
            _headers[key] = value;
        
    }
    
    [PublicAPI]
    public RequestBuilder SetRequestMethod(RequestMethodType type)
    {
        _type = type;
        return this;
    }

    [PublicAPI]
    public RequestBuilder SetUri(CompleteUri uri)
    {
        _uri = uri;
        return this;
    }

    [PublicAPI]
    public RequestBuilder SetUri(string uri)
    {
        _uri = new CompleteUri(uri);
        return this;
    }
    
    [PublicAPI]
    public RequestBuilder AddHeader(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    [PublicAPI]
    public RequestBuilder SetBody(string body)
    {
        _body = body;
        return this;
    }

    [PublicAPI]
    public (CompleteUri uri, RequestEntity entity) Build()
        => (_uri, new RequestEntity(_type, _uri, _body));
}