using JetBrains.Annotations;

#if DEBUG

namespace MemwLib.Data.Json.Exceptions;

/// <summary>Thrown when an unexpected character was found.</summary>
/// <remarks>This class is not constructable externally.</remarks>
[PublicAPI]
public class UnexpectedJsonEoiException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    internal UnexpectedJsonEoiException(char found, string expected, int index) 
        : this(found, new []{ expected }, index) {}
    
    internal UnexpectedJsonEoiException(char found, string[] expected, int index)
    {
        Message = $"Invalid character found '{found}' in index {index}, expected: {string.Join(" or ", expected.Select(c => c.Length == 1 ? $"'{c}'" : $"\"{c}\""))}.";
    }
}

#endif