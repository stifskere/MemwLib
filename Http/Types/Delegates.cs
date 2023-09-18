using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Logging;

namespace MemwLib.Http.Types;

public delegate ResponseEntity RequestDelegate(RequestEntity request);
public delegate void LogDelegate(LogMessage message);