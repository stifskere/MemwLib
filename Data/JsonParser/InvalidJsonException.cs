#if DEBUG

namespace MemwLib.Data.JsonParser;

internal class InvalidJsonException : Exception
{
    public override string Message { get; }

    public InvalidJsonException(InvalidJsonExceptionType type, char token, int index)
    {
        Message = type switch
        {
            InvalidJsonExceptionType.InvalidCharacter => $"Invalid character found '{token}' in index {index}.",
            InvalidJsonExceptionType.MissingTrailing => $"Missing '{token}' for closure starting at {index}.",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Exception type out of range.")
        };
    }
}

internal enum InvalidJsonExceptionType
{
    MissingTrailing,
    InvalidCharacter
}

#endif