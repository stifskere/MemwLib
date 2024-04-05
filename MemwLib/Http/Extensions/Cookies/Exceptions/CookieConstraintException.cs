namespace MemwLib.Http.Extensions.Cookies.Exceptions;

/// <summary>Thrown when invalid cookie manipulation is done.</summary>
/// <param name="reason">The throw reason.</param>
public class CookieConstraintException(string reason) : Exception
{
    /// <inheritdoc />
    public override string Message => reason;
}