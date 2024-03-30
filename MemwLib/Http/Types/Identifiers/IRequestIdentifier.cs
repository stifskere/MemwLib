namespace MemwLib.Http.Types.Identifiers;

internal interface IRequestIdentifier
{
    public RequestMethodType RequestMethod { get; }
}