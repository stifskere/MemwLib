using JetBrains.Annotations;
using MemwLib.Http.Types.Entities;

namespace MemwLib.Http.Types.Attributes;

/// <summary>
/// Base middleware class, every class extending this
/// one will be counted as middleware within the HTTPServer
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public abstract class MiddlewareAttribute : Attribute
{
    /// <summary>
    /// This method will be ran every time a route
    /// or route group with this middleware is called.
    /// </summary>
    /// <param name="request">The incoming request from the server.</param>
    /// <returns>
    /// A ResponseEntity to end the request or a NextMiddleware instance
    /// telling the server to call the next corresponding handler for this request.
    /// </returns>
    public abstract IResponsible Handler(RequestEntity request);
}