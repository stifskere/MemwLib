namespace MemwLib.Http.Types.Content.Views.Exceptions;

public class InvalidArithmeticOperatorException(string invalid) : Exception
{
    public override string Message => $"Invalid operator: {invalid}";
}