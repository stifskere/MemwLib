using JetBrains.Annotations;

namespace MemwLib.Data.Json.Exceptions;

/// <summary>Thrown when a constraint for JSON format was broken.</summary>
/// <remarks>This class is not constructable externally.</remarks>
[PublicAPI]
public class InvalidJsonConstraintException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal InvalidJsonConstraintException(string message, int? line = null)
    {
        Message = message;
    }
}