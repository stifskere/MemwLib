using System.Data;
using System.Reflection;
using JetBrains.Annotations;

namespace MemwLib.Data.ArgumentParser.Exceptions;

/// <summary>Thrown when an argument couldn't be converted successfully</summary>
[PublicAPI]
public sealed class ConvertArgumentException : Exception
{
    /// <inheritdoc cref="Exception.Message"/>
    public override string Message { get; }
    
    /// <summary>The property that was supposed to be set.</summary>
    public PropertyInfo TargetProperty { get; }
    
    /// <summary>The value that was supposed to be set to the target property.</summary>
    public string Value { get; }
    
    /// <summary>The internal exception thrown by the converter.</summary>
    public Exception InternalException { get; }
    
    internal ConvertArgumentException(PropertyInfo property, string value, Exception inner)
    {
        Message = $"Value \"{value}\" is not convertible to type {property.PropertyType.Name} for property {property.Name}.";
        TargetProperty = property;
        Value = value;
        InternalException = inner;
    }
}