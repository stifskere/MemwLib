using JetBrains.Annotations;

namespace MemwLib.Data.Json.Enums;

/// <summary>Enum used to define the behavior of a type acting as a json object.</summary>
[PublicAPI]
public enum JsonObjectBehavior
{
    /// <summary>This will only fill the properties that have the JsonPropertyAttribute.</summary>
    /// <exception cref="NullReferenceException"
    /// >A non nullable property that didn't
    /// have the JsonPropertyAttribute wasn't filled.
    /// </exception>
    OnlyPropertiesWithAttribute = 1,
    
    /// <summary>Fills all properties that are found in the object.</summary>
    /// <exception cref="NullReferenceException">
    /// A non nullable property present
    /// in the type was not found in the target object.
    /// </exception>
    /// <remarks>
    /// This is the default value, if you consider using this value
    /// you might as well remove the JsonObjectAttribute from your class.
    /// </remarks>
    AllProperties = 1 << 1
}