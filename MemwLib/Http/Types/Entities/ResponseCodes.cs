using JetBrains.Annotations;
using MemwLib.Strings;

namespace MemwLib.Http.Types.Entities;

/// <summary>
/// Represents HTTP status codes as an enumeration for better readability and usage.
/// Each enum member corresponds to a specific HTTP status code along with its associated description.
/// </summary>
[PublicAPI]
public enum ResponseCodes
{
    /// <summary>
    /// The server has received the request headers and the client should proceed to send the request body.
    /// </summary>
    Continue = 100,

    /// <summary>
    /// The requester has asked the server to switch protocols and the server has agreed to do so.
    /// </summary>
    SwitchingProtocols = 101,
    
    /// <summary>
    /// indicates that the server has received and is processing the request, but no response is available yet.
    /// </summary>
    Processing = 102,
    
    /// <summary>
    /// This status code is primarily intended to be used with the Link header,
    /// letting the user agent start preloading resources while the server prepares
    /// a response or pre-connect to an origin from which the page will need resources.
    /// </summary>
    EarlyHints = 103,
    
    /// <summary>
    /// Standard response for successful HTTP requests.
    /// </summary>
    Ok = 200,

    /// <summary>
    /// The request has been fulfilled, resulting in the creation of a new resource.
    /// </summary>
    Created = 201,

    /// <summary>
    /// The request has been accepted for processing, but the processing has not been completed.
    /// </summary>
    Accepted = 202,

    /// <summary>
    /// The server successfully processed the request but returned non-authoritative information.
    /// </summary>
    NonAuthoritativeInformation = 203,

    /// <summary>
    /// The server successfully processed the request but there is no additional information to send back.
    /// </summary>
    NoContent = 204,

    /// <summary>
    /// The server successfully processed the request but wants to instruct the client to reset the document view.
    /// </summary>
    ResetContent = 205,

    /// <summary>
    /// The server has fulfilled the partial GET request for the resource.
    /// </summary>
    PartialContent = 206,

    /// <summary>
    /// Conveys information about multiple resources, for situations where multiple status codes might be appropriate.
    /// </summary>
    MultiStatus = 207,
    
    /// <summary>
    /// Used inside a &lt;dav:propstat&gt; response element to avoid repeatedly enumerating the
    /// internal members of multiple bindings to the same collection.
    /// </summary>
    AlreadyReported = 208,
    
    /// <summary>
    /// The server has fulfilled a GET request for the resource, and the response is a representation of
    /// the result of one or more instance-manipulations applied to the current instance.
    /// </summary>
    ImUsed = 226,
    
    /// <summary>
    /// The requested resource corresponds to any one of a set of representations, each with its own specific location.
    /// </summary>
    MultipleChoices = 300,

    /// <summary>
    /// The requested resource has been assigned a new permanent URI and any future references to this resource should use one of the returned URIs.
    /// </summary>
    MovedPermanently = 301,

    /// <summary>
    /// The requested resource can be found under a different URI.
    /// </summary>
    Found = 302,

    /// <summary>
    /// The response to the request can be found under a different URI and should be retrieved using a GET method on that resource.
    /// </summary>
    SeeOther = 303,

    /// <summary>
    /// The server has not modified the document, but there is no information to send back.
    /// </summary>
    NotModified = 304,

    /// <summary>
    /// The requested resource temporarily resides under a different URI.
    /// </summary>
    TemporaryRedirect = 307,

    /// <summary>
    /// The requested resource resides permanently under a different URI.
    /// </summary>
    PermanentRedirect = 308,
    
    /// <summary>
    /// The server cannot or will not process the request due to an apparent client error.
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// Similar to 401 (Unauthorized), but indicates that the client must authenticate itself to get permission.
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// Reserved for future use. The original intention was that this code might indicate that the client must make payment to access the resource.
    /// </summary>
    PaymentRequired = 402,

    /// <summary>
    /// The client does not have access rights to the content, i.e., they are unauthorized to access the requested resource.
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// The server can not find the requested resource. This status code is often used as a catch-all for all methods for which no specific status code is applicable.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// The method specified in the request is not allowed for the resource identified by the request URI.
    /// </summary>
    MethodNotAllowed = 405,

    /// <summary>
    /// The requested resource is capable of generating only content not acceptable according to the Accept headers sent in the request.
    /// </summary>
    NotAcceptable = 406,

    /// <summary>
    /// Similar to 401 (Unauthorized), but indicates that the client must first authenticate itself with the proxy.
    /// </summary>
    ProxyAuthenticationRequired = 407,

    /// <summary>
    /// The server timed out waiting for the request.
    /// </summary>
    RequestTimeout = 408,

    /// <summary>
    /// Indicates that the request could not be processed because of conflict in the request.
    /// </summary>
    Conflict = 409,

    /// <summary>
    /// Indicates that the requested resource is no longer available at the server and no forwarding address is known.
    /// </summary>
    Gone = 410,

    /// <summary>
    /// The server refuses to accept the request without a defined Content-Length.
    /// </summary>
    LengthRequired = 411,

    /// <summary>
    /// The precondition given in one or more of the request-header fields evaluated to false when it was tested on the server.
    /// </summary>
    PreconditionFailed = 412,

    /// <summary>
    /// The server is refusing to process a request because the request entity is larger than the server is willing or able to process.
    /// </summary>
    RequestEntityTooLarge = 413,

    /// <summary>
    /// The server is refusing to service the request because the request-URI is longer than the server is willing to interpret.
    /// </summary>
    RequestUriTooLong = 414,

    /// <summary>
    /// The server is refusing to service the request because the entity of the request is in a format not supported by the requested resource for the requested method.
    /// </summary>
    UnsupportedMediaType = 415,

    /// <summary>
    /// The client has asked for a portion of the file, but the server cannot supply that portion.
    /// </summary>
    RequestedRangeNotSatisfiable = 416,

    /// <summary>
    /// The server cannot meet the requirements of the Expect request-header field.
    /// </summary>
    ExpectationFailed = 417,

    /// <summary>
    /// indicates that the server refuses to brew coffee because it is, permanently, a teapot. A combined coffee/tea pot that is
    /// temporarily out of coffee should instead return 503. This error is a reference to Hyper Text Coffee Pot Control Protocol defined
    /// in April Fools' jokes in 1998 and 2014.
    /// </summary>
    ImATeaPot = 418,
    
    /// <summary>
    /// The request was directed at a server that is not able to produce a response. This can be sent by a server that is not configured to produce responses
    /// for the combination of scheme and authority that are included in the request URI. 
    /// </summary>
    MisdirectedRequest = 421,
    
    /// <summary>
    /// The request was well-formed but was unable to be followed due to semantic errors.
    /// </summary>
    UnprocessableContent = 422,
    
    /// <summary>
    /// The resource that is being accessed is locked.
    /// </summary>
    Locked = 423,
    
    /// <summary>
    /// The request failed due to failure of a previous request.
    /// </summary>
    FailedDependency = 424,
    
    /// <summary>
    /// Indicates that the server is unwilling to risk processing a request that might be replayed.
    /// </summary>
    TooEarly = 425,
    
    /// <summary>
    /// The server refuses to perform the request using the current protocol but might be willing to do so
    /// after the client upgrades to a different protocol. The server sends an Upgrade header in a 426 response
    /// to indicate the required protocol(s). 
    /// </summary>
    UpgradeRequired = 426,
    
    /// <summary>
    /// The origin server requires the request to be conditional. This response is intended to prevent the
    /// 'lost update' problem, where a client GETs a resource's state, modifies it and PUTs it back to the
    /// server, when meanwhile a third party has modified the state on the server, leading to a conflict. 
    /// </summary>
    PreconditionRequired = 428,
    
    /// <summary>
    /// indicates the user has sent too many requests in a given amount of time.
    /// </summary>
    TooManyRequests = 429,
    
    /// <summary>
    /// The server is unwilling to process the request because its header fields are too large.
    /// The request may be resubmitted after reducing the size of the request header fields. 
    /// </summary>
    RequestFieldsTooLarge = 431,
    
    /// <summary>
    /// The user agent requested a resource that cannot legally be provided, such as a web page censored by a government.
    /// </summary>
    UnavailableForLegalReasons = 451,
    
    /// <summary>
    /// A generic error message returned when an unexpected condition was encountered on the server.
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    /// The server either does not recognize the request method, or it lacks the ability to fulfill the request.
    /// </summary>
    NotImplemented = 501,

    /// <summary>
    /// The server was acting as a gateway or proxy and received an invalid response from the upstream server.
    /// </summary>
    BadGateway = 502,

    /// <summary>
    /// The server is currently unable to handle the request due to temporary overloading or maintenance of the server.
    /// </summary>
    ServiceUnavailable = 503,

    /// <summary>
    /// The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server or some other auxiliary server it needed to access in order to complete the request.
    /// </summary>
    GatewayTimeout = 504,

    /// <summary>
    /// The server does not support the HTTP protocol version that was used in the request.
    /// </summary>
    VersionNotSupported = 505,
    
    /// <summary>
    /// The server has an internal configuration error: the chosen variant resource is configured to engage in
    /// transparent content negotiation itself, and is therefore not a proper end point in the negotiation process.
    /// </summary>
    VariantAlsoNegotiates = 506,
    
    /// <summary>
    /// The method could not be performed on the resource because the server is unable to store the representation needed
    /// to successfully complete the request.
    /// </summary>
    InsufficientStorage = 507,
    
    /// <summary>
    /// The server detected an infinite loop while processing the request.
    /// </summary>
    LoopDetected = 508,
    
    /// <summary>
    /// Further extensions to the request are required for the server to fulfill it.
    /// </summary>
    NotExtended = 510,
    
    /// <summary>
    /// Indicates that the client needs to authenticate to gain network access.
    /// </summary>
    NetworkAuthenticationRequired = 511
}

/// <summary>Extension methods for the response codes enum instances.</summary>
[PublicAPI]
public static class ResponseCodesExtensions
{
    /// <summary>Obtain a formatted string representing the response code hint.</summary>
    /// <param name="code">The ResponseCode enumerable instance.</param>
    /// <returns>The formatted response code as string.</returns>
    public static string GetName(this ResponseCodes code)
        => code.ToString().Separate();

    /// <summary>Obtain the response code as integer.</summary>
    /// <param name="code">The ResponseCode enumerable instance.</param>
    /// <returns>The response code as integer.</returns>
    public static int GetCode(this ResponseCodes code)
        => (int)code;
}