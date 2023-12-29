using JetBrains.Annotations;

namespace MemwLib.Http.Types.Entities;

/// <summary>Tells the server to execute the next middleware piece.</summary>
/// <remarks>This class contains nothing, it's purely used for type metadata.</remarks>
[PublicAPI]
public class NextMiddleWare : IResponsible { }