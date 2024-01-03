using System.Collections;
using JetBrains.Annotations;

namespace MemwLib.CoreUtils.Collections;

/// <summary>Abstract class to define isolated implementations.</summary>
/// <typeparam name="TKey">The type of the keys for this collection instance.</typeparam>
/// <typeparam name="TValue">The type of the values for this collection instance.</typeparam>
[PublicAPI]
public abstract class BaseIsolatedCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    /// <summary>Collection default dictionary</summary>
    /// <remarks>This should not be exposed.</remarks>
    protected readonly Dictionary<TKey, TValue> Variables = new();
    
    /// <summary>How many variables exist in this collection.</summary>
    public virtual int Length => Variables.Count;

    /// <summary>Initializes an empty instance.</summary>
    protected BaseIsolatedCollection() {}
    
    /// <summary>Initializes a collection instance with another collection's items.</summary>
    /// <param name="collection">The collection to get the items from.</param>
    protected BaseIsolatedCollection(BaseIsolatedCollection<TKey, TValue> collection)
    {
        foreach ((TKey key, TValue value) in collection)
            Variables[key] = value;
    }
    
    /// <summary>Checks if there is a variable with the specified key.</summary>
    /// <param name="key">The key to check</param>
    /// <returns>true if the variable exists, otherwise false.</returns>
    public virtual bool Contains(TKey key)
        => Variables.ContainsKey(key);

    /// <summary>Sets a value in this collection.</summary>
    /// <param name="key">The key that references the object.</param>
    /// <param name="value">The value itself.</param>
    /// <remarks>If the value already existed in that key it will be replaced.</remarks>
    public virtual void Set(TKey key, TValue value)
    {
        Variables[key] = value;
    }

    /// <summary>Gets a value from this collection.</summary>
    /// <param name="key">The key that references the object.</param>
    /// <returns>The value that was referenced by the key, or null if it did not exist.</returns>
    public virtual TValue? Get(TKey key)
        => Variables.GetValueOrDefault(key);

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Remove(TKey)"/>
    public virtual void Remove(TKey key)
    {
        Variables.Remove(key);
    }

    /// <inheritdoc cref="Dictionary{TKey,TValue}.this[TKey]"/>
    public virtual TValue this[TKey key] {
        get => Variables[key];
        set => Set(key, value);
    }
    
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() 
        => Variables.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}