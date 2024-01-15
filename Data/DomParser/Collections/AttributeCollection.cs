#if DEBUG

using JetBrains.Annotations;
using MemwLib.CoreUtils.Collections;
using MemwLib.Data.DomParser.Exceptions;
using MemwLib.Data.DomParser.Structures;

namespace MemwLib.Data.DomParser.Collections;

/// <summary>Represents a collection of attributes for an HTMLElement</summary>
[PublicAPI]
public class AttributeCollection : BaseIsolatedCollection<string, string>
{
    /// <inheritdoc cref="BaseIsolatedCollection{TKey,TValue}.Set"/>
    /// <exception cref="InvalidElementException">Thrown when the attribute name doesn't match /[a-z\-]+/</exception>
    public override void Set(string key, string value)
    {
        if (!HtmlElement.ValidTagRegex().IsMatch(key))
            throw new InvalidElementException($"The attribute {key} has not a valid name.");
        
        base.Set(key, value);
    }
}

#endif