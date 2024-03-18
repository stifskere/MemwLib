namespace MemwLib.Http.Types.Content.Views.Exceptions;

public class InvalidExpressionException(Location location, string reason) : Exception
{
    public override string Message => $"Invalid sequence found in {location}; {reason}";
}