using System.Collections;
using JetBrains.Annotations;

namespace MemwLib.CoreUtils.Collections;

// TODO: rewrite XML
// TODO: limit the parameter number for possible single key value map implementations.

/// <summary>Abstract class to define isolated implementations.</summary>
/// <typeparam name="TKey">The type of the keys for this collection instance.</typeparam>
/// <typeparam name="TValue">The type of the values for this collection instance.</typeparam>
[PublicAPI]
public abstract class BaseIsolatedCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, ICountable 
    where TKey : notnull
{
    /// <summary>Collection default dictionary</summary>
    /// <remarks>This should not be exposed.</remarks>
    protected readonly List<KeyValuePair<TKey, TValue>> Variables = [];
    
    /// <summary>How many variables exist in this collection.</summary>
    public virtual int Length => Variables.Count;

    /// <inheritdoc />
    public virtual bool IsEmpty => Length == 0;

    private ILookup<TKey, TValue> AsLookup => Variables.ToLookup(
        pair => pair.Key,
        pair => pair.Value
    );
    
    /// <summary>Initializes an empty instance.</summary>
    protected BaseIsolatedCollection() {}
    
    /// <summary>Initializes a collection instance with another collection's items.</summary>
    /// <param name="collection">The collection to get the items from.</param>
    protected BaseIsolatedCollection(BaseIsolatedCollection<TKey, TValue> collection)
    {
        foreach ((TKey key, TValue value) in collection)
            Variables.Add(new KeyValuePair<TKey, TValue>(key, value));
    }
    
    /// <summary>Checks if there is a variable with the specified key.</summary>
    /// <param name="key">The key to check</param>
    /// <returns>true if the variable exists, otherwise false.</returns>
    public virtual bool Contains(TKey key)
        => AsLookup.Contains(key);

    /// <summary>Sets a value in this collection.</summary>
    /// <param name="key">The key that references the object.</param>
    /// <param name="values">The value itself.</param>
    /// <remarks>If the value already existed in that key it will be replaced.</remarks>
    /// <returns>The same instance to act as a constructor.</returns>
    public virtual BaseIsolatedCollection<TKey, TValue> Set(TKey key, params TValue[] values)
    {
        foreach (TValue value in values)
            Variables.Add(new KeyValuePair<TKey, TValue>(key, value));
        
        return this;
    }

    /// <summary>Grab properties from another collection and adds them to this collection.</summary>
    /// <param name="other">The other collection</param>
    /// <remarks>If the value already existed in that key it will be replaced.</remarks>
    /// <returns>The same instance to act as a constructor.</returns>
    public virtual BaseIsolatedCollection<TKey, TValue> Add(BaseIsolatedCollection<TKey, TValue> other)
    {
        foreach ((TKey key, TValue value) in other)
            Set(key, value);

        return this;
    }

    /// <summary>Grab properties from another collection and adds them to this collection.</summary>
    /// <param name="other">The other collection</param>
    /// <remarks>If the value already existed in that key it will be replaced.</remarks>
    /// <returns>The same instance to act as a constructor.</returns>
    public virtual BaseIsolatedCollection<TKey, TValue> Add(Dictionary<TKey, TValue> other)
    {
        foreach ((TKey key, TValue value) in other)
            Set(key, value);

        return this;
    }

    /// <summary>Gets a value from this collection.</summary>
    /// <param name="key">The key that references the object.</param>
    /// <returns>The value that was referenced by the key, or null if it did not exist.</returns>
    public virtual TValue[] Get(TKey key)
        => Variables
            .Where(pair => pair.Key.Equals(key))
            .Select(pair => pair.Value)
            .ToArray();

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Remove(TKey)"/>
    public virtual void Remove(TKey key)
    {
        Variables
            .RemoveAll(pair => pair.Key.Equals(key));
    }

    /// <inheritdoc cref="Dictionary{TKey,TValue}.this[TKey]"/>
    public virtual TValue[] this[TKey key]
    {
        get => Get(key);
        set => Set(key, value);
    }
    
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() 
        => Variables.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}