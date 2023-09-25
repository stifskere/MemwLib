namespace MemwLib.Http.Types.Identifiers;

internal class StringRequestIdentifier : IRequestIdentifier
{
    public required RequestMethodType RequestType { get; init; }
    public required string Path { get; init; }
}