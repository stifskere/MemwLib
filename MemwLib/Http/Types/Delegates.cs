using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Logging;

namespace MemwLib.Http.Types;

/// <summary>The delegate used for HTTPServer endpoint handler.</summary>
/// <param name="request">The request sent by the client.</param>
/// <returns>The response that the server should send.</returns>
public delegate ResponseEntity RequestDelegate(RequestEntity request);

/// <summary>The delegate used for Logging in the HTTPServer.</summary>
/// <param name="message">The message that the server returned.</param>
public delegate void LogDelegate(LogMessage message);

/// <summary>Delegate used to implement custom logic to HTTP requests within the HTTPServer.</summary>
/// <param name="request">The message sent by the client.</param>
/// <returns>The response that the server should send.</returns>
/// <remarks>
/// Returning a response after calling the next method will result in ignoring that response,
/// so you might want to simply return null after the call to the next function.
/// </remarks>
public delegate IResponsible MiddleWareDelegate(RequestEntity request);

/// <summary>Delegate used to implement response code interception logic.</summary>
/// <param name="response">The anterior response.</param>
/// <returns>The response that the server should send.</returns>
/// <remarks>Changing the response code won't trigger other interceptors.</remarks>
public delegate IResponsible InterceptorDelegate(ResponseEntity response);