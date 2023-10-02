using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Entities;

namespace MemwLib.Http.Types.Builders;
using Uri = MemwLib.Http.Types.Routes.Uri;

public class RequestBuilder
{
    private RequestMethodType _type;
    private Uri _uri;
    private string? _body;
    private readonly HeaderCollection _headers = new();
    
    public RequestBuilder(RequestMethodType type, Uri uri, string? body = null, (string key, string value)[]? headers = null)
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
    public (Uri uri, RequestEntity entity) Build()
        => (_uri, new RequestEntity(_type, _uri, _body));
}