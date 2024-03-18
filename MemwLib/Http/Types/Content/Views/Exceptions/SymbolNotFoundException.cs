namespace MemwLib.Http.Types.Content.Views.Exceptions;

public class SymbolNotFoundException(string symbol, Location location) : Exception
{
    public override string Message => $"Symbol not found {symbol} in {location}";
}