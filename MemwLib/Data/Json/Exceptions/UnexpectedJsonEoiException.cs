using JetBrains.Annotations;

namespace MemwLib.Data.Json.Exceptions;

/// <summary>Thrown when an unexpected character was found.</summary>
/// <remarks>This class is not constructable externally.</remarks>
[PublicAPI]
public class UnexpectedJsonEoiException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal UnexpectedJsonEoiException(char found, char expected, int index)
    {
        Message = $"Invalid character found '{found}' in index {index}, expected: '{expected}'.";
    }
}