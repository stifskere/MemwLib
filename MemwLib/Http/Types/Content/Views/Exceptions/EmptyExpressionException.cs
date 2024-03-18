namespace MemwLib.Http.Types.Content.Views.Exceptions;

public class EmptyExpressionException(Location location) : Exception
{
    public override string Message => $"Empty expressions are not allowed. Found in {location}";
}