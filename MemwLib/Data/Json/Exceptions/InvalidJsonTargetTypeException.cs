#if DEBUG

namespace MemwLib.Data.Json.Exceptions;

/// <summary>Thrown when the target type for a JSON payload didn't match.</summary>
/// <remarks>This class is not constructable externally.</remarks>
public class InvalidJsonTargetTypeException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal InvalidJsonTargetTypeException(string name)
    {
        Message = $"Invalid JSON object for target type <{name}>";
    }
}

#endif