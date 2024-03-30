namespace MemwLib.Http.Types.Exceptions;

/// <summary>Thrown when a middleware target method is not found or not eligible.</summary>
public sealed class MiddleWareNotEligibleException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal MiddleWareNotEligibleException(Type type, string methodName)
    {
        Message = $"A middleware target not found as {methodName} in {type.Name}";
    }
}