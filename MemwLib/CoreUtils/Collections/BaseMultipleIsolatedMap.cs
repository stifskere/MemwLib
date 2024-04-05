using System.Collections;

namespace MemwLib.CoreUtils.Collections;

/// <summary>Abstract class to define isolated map implementations where a key can map to multiple values.</summary>
/// <typeparam name="TKey">The type of the keys for this collection instance.</typeparam>
/// <typeparam name="TValue">The type of the values for this collection instance.</typeparam>
public abstract class BaseMultipleIsolatedMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, ICountable 
    where TKey : notnull 
{
    /// <summary>Collection default dictionary</summary>
    /// <remarks>This should not be exposed.</remarks>
    protected readonly List<KeyValuePair<TKey, TValue>> Variables = [];

    private ILookup<TKey, TValue> AsLookup => Variables
        .ToLookup(
            pair => pair.Key,
            pair => pair.Value
        );
    
    /// <summary>The total values count, counting including keys with multiple values.</summary>
    public virtual int Length => Variables.Count;

    /// <summary>The total keys count.</summary>
    public virtual int KeyLength => AsLookup.Count;
    
    /// <inheritdoc/>
    public virtual bool IsEmpty => Length == 0;

    /// <summary>Initializes an empty instance.</summary>
    protected BaseMultipleIsolatedMap() {}
    
    /// <summary>Initializes a collection instance with another collection's items.</summary>
    /// <param name="collection">The collection to get the items from.</param>
    protected BaseMultipleIsolatedMap(BaseIsolatedMap<TKey, TValue> collection)
    {
        foreach ((TKey key, TValue value) in collection)
            Variables.Add(new KeyValuePair<TKey, TValue>(key, value)); // avoid virtual method in constructor call.
    }
    
    /// <summary>Checks if there is a variable set with the specified key.</summary>
    /// <param name="key">The key to check.</param>
    /// <returns>true if the key maps to something, otherwise false.</returns>
    public virtual bool Contains(TKey key)
        => AsLookup.Contains(key);

    /// <summary>Adds a value to this collection.</summary>
    /// <param name="key">The key that references the object.</param>
    /// <param name="value">The value itself.</param>
    /// <returns>The same instance to act as a constructor.</returns>
    public virtual BaseMultipleIsolatedMap<TKey, TValue> Add(TKey key, TValue value)
    {
        Variables.Add(new KeyValuePair<TKey, TValue>(key, value));
        return this;
    }

    /// <summary>Grab properties from another collection and adds them to this collection.</summary>
    /// <param name="other">The other collection.</param>
    /// <returns>The same instance to act as a constructor.</returns>
    public virtual BaseMultipleIsolatedMap<TKey, TValue> Add(BaseIsolatedMap<TKey, TValue> other)
    {
        foreach ((TKey key, TValue value) in other)
            Add(key, value);

        return this;
    }

    /// <summary>Grab properties from another collection and adds them to this collection.</summary>
    /// <param name="other">The other collection.</param>
    /// <returns>The same instance to act as a constructor.</returns>
    public virtual BaseMultipleIsolatedMap<TKey, TValue> Add(Dictionary<TKey, TValue> other)
    {
        foreach ((TKey key, TValue value) in other)
            Add(key, value);

        return this;
    }

    /// <summary>Gets a set of values from this collection.</summary>
    /// <param name="key">The key that references the object.</param>
    /// <returns>An array with all the values mapped to that key, null if the key was not found.</returns>
    public virtual TValue[]? Get(TKey key)
        => Contains(key) ? AsLookup[key].ToArray() : null;

    /// <summary>Removes all values mapped to a key and the key itself</summary>
    /// <param name="key">The key thus values are mapped.</param>
    public virtual void Remove(TKey key)
    {
        Variables.RemoveAll(pair => pair.Key.Equals(key));
    }

    /// <summary>Get all the values mapped to the specified key as a TValue[]</summary>
    /// <param name="key">
    /// The key that maps to the wanted values,
    /// setting to null will delete the key and it's values.
    /// </param>
    public virtual TValue[]? this[TKey key]
    {
        get => Get(key);

        set
        {
            if (value is null)
            {
                Remove(key);
                return;
            }
            
            foreach (TValue val in value)
                Add(key, val);
        }
    }
    
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() 
        => Variables.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}