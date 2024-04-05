using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.CoreUtils.Collections;

namespace MemwLib.Http.Types.Collections;

/// <summary>A collection implementation for capturing groups found in URLs in routes.</summary>
/// <remarks>The constructor for this collection is internal.</remarks>
[PublicAPI]
public sealed class HttpRegexGroupCollection : BaseIsolatedMap<string, string>
{
    internal HttpRegexGroupCollection() {}
    
    internal void Add(GroupCollection collection)
    {
        foreach (Group group in collection)
            if (group.Name != 0.ToString())
                Set(group.Name, group.ToString());
    }

    /// <summary>Indexer to get a specific group from the current route.</summary>
    /// <param name="index">The index of the group, if named a string otherwise an integer.</param>
    /// <example>For groups like (?&lt;name&gt;\d) use the string indexer otherwise if the group is conventional use the integer indexer.</example>
    public override string this[string index] => Variables[index];

    /// <inheritdoc cref="HttpRegexGroupCollection.this[string]"/>
    public string this[int index] => index > 0
        ? Variables[index.ToString()]
        : throw new ArgumentOutOfRangeException(nameof(index),
            index > Length
                ? "The supplied index is out of range."
                : "Regex indexes start by 1."
        );
}