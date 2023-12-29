using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

/// <summary>A collection implementation for capturing groups found in URLs in routes.</summary>
/// <remarks>The constructor for this collection is internal.</remarks>
[PublicAPI]
public sealed class HttpRegexGroupCollection : BaseIsolatedCollection<string, string>
{
    private readonly Dictionary<string, Group> _matches = new();
    
    internal HttpRegexGroupCollection() {}
    
    internal void Add(GroupCollection collection)
    {
        foreach (Group group in collection)
            if (group.Name != 0.ToString())
                _matches.Add(group.Name, group);
    }

    /// <summary>Indexer to get a specific group from the current route.</summary>
    /// <param name="index">The index of the group, if named a string otherwise an integer.</param>
    /// <example>For groups like (?&lt;name&gt;\d) use the string indexer otherwise if the group is conventional use the integer indexer.</example>
    public override string this[string index] => _matches[index].Value;

    /// <inheritdoc cref="HttpRegexGroupCollection.this[string]"/>
    public string this[int index] => _matches[index.ToString()].Value;
}