namespace MemwLib.Http.Types.Content.Views.Exceptions;

public class InvalidReferenceNameException(string name) : Exception
{
    public override string Message => $"Invalid reference name \"{name}\"";
}