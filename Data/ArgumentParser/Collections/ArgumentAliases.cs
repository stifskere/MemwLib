using System.Collections;
using JetBrains.Annotations;

namespace MemwLib.Data.ArgumentParser.Collections;

/// <summary>Aliases for dynamic arguments.</summary>
[PublicAPI]                                                                                              
public sealed class ArgumentAliases : IEnumerable<KeyValuePair<string, string>>
{
    private readonly Dictionary<string, string> _aliases = new();

    /// <summary>Adds an alias to this collection.</summary>
    /// <returns>The same instance.</returns>
    public ArgumentAliases AddAlias(string name, string alias)
    {
        _aliases.Add(alias, name);
        return this;
    }

    internal string? FindName(string alias) 
        => _aliases.GetValueOrDefault(alias);

    internal int Length => _aliases.Count;

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        foreach ((string alias, string name) in _aliases)
            yield return new KeyValuePair<string, string>(name, alias);
    }

    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();

    /// <summary>Index a property name by alias.</summary>
    /// <param name="alias">The alias to find the name of.</param>
    public string this[string alias]
    {
        get => _aliases[alias];
        internal set => _aliases[alias] = value;
    }
}