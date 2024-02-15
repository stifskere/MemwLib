#if DEBUG

using JetBrains.Annotations;
using MemwLib.Data.Json.Enums;

namespace MemwLib.Data.Json.Attributes;

/// <summary>Attribute used to define the behavior of a JSON parser on a type.</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class), PublicAPI]
public class JsonObjectAttribute : Attribute
{
    internal JsonObjectBehavior Behavior { get; }
    
    /// <summary>
    /// Initializes a new instance of JsonObjectAttribute
    /// to define the behavior of the parser on the type.
    /// </summary>
    /// <param name="behavior">the defined behavior.</param>
    public JsonObjectAttribute(JsonObjectBehavior behavior)
    {
        Behavior = behavior;
    }
}

#endif