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