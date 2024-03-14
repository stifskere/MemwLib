using JetBrains.Annotations;

namespace MemwLib.Data.Json.Attributes;

/// <summary>
/// This class defines the name of the json property in
/// the object that should be assigned to this type's property.
/// </summary>
[AttributeUsage(AttributeTargets.Property), PublicAPI]
public class JsonPropertyAttribute : Attribute
{
    internal string? Name { get; }
    
    /// <summary>
    /// Initializes a new instance of JsonPropertyAttribute
    /// to define the name of the property.
    /// </summary>
    /// <param name="name">The related name on the JSON object.</param>
    /// <remarks>
    /// If the name parameter was not provided this
    /// will only instruct the parser to include this property
    /// if the object behavior was defined to be OnlyPropertiesWithAttribute,
    /// and search the property name in the JSON object which will most
    /// likely not be found (if following json naming conventions).
    /// </remarks>
    public JsonPropertyAttribute(string? name = null)
    {
        Name = name;
    }
}