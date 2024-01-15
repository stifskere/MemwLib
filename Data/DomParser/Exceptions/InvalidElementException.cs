#if DEBUG

namespace MemwLib.Data.DomParser.Exceptions;

/// <summary>
/// Exception thrown when there was a
/// problem on setting a property within an HTML element.
/// </summary>
public class InvalidElementException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal InvalidElementException(string message)
    {
        Message = message;
    }
}

#endif