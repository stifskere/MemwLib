namespace MemwLib.Http.Types.Identifiers;

internal interface IRequestIdentifier
{
    public RequestMethodType RequestType { get; init; }
}