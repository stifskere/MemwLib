namespace MemwLib.Data.Json.Exceptions;

/// <summary>Thrown when an invalid sequence start or end is found</summary>
/// <remarks>This class is not constructable externally.</remarks>
public class InvalidJsonSequenceException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal InvalidJsonSequenceException(char enclosedBy)
    {
        Message = $"Invalid sequence enclosing for JSON, expected to be enclosed by '{enclosedBy}'";
    }

    internal InvalidJsonSequenceException(char start, char end)
    {
        Message = $"Invalid sequence enclosing for JSON, expected to start with '{start}' and end with '{end}'";
    }
}