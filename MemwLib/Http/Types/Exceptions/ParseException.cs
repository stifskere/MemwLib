using MemwLib.Http.Types.Entities;

namespace MemwLib.Http.Types.Exceptions;

/// <summary>This exception is thrown when an entity couldn't be parsed.</summary>
public class ParseException<T> : Exception where T : BaseEntity
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; } = $"An error occurred while parsing an instance of <{typeof(T).Name}>.";
}