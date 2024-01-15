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
    /// The requested resource must be accessed through a proxy, specified in the Location header.
    /// </summary>
    UseProxy = 305,

    /// <summary>
    /// The requested resource temporarily resides under a different URI.
    /// </summary>
    TemporaryRedirect = 307,

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
    VersionNotSupported = 505
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