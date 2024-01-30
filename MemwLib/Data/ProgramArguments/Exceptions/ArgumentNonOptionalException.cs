using System.Reflection;
using JetBrains.Annotations;

namespace MemwLib.Data.ProgramArguments.Exceptions;

/// <summary>Thrown when a value is not found for a non optional property.</summary>
[PublicAPI]
public sealed class ArgumentNonOptionalException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }

    /// <summary>The property that was supposed to be set.</summary>
    public PropertyInfo TargetProperty { get; }

    internal ArgumentNonOptionalException(PropertyInfo property)
    {
        Message = $"An argument definition for {property.Name} was not found, and property is not optional.";
        TargetProperty = property;
    }
}