using System.Collections;
using JetBrains.Annotations;
using DisallowNull = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;

namespace MemwLib.Http.Types.Collections;

/// <summary>Abstract class for base collection, contains common fields between all the collection types.</summary>
public abstract class BaseCollection : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>Collection default dictionary</summary>
    /// <remarks>This should not be exposed.</remarks>
    protected readonly Dictionary<string, string> Variables = new();

    /// <summary>Checks if there is a variable with the specified key.</summary>
    /// <param name="key">The key to check</param>
    /// <returns>true if the variable exists, otherwise false.</returns>
    [PublicAPI]
    public bool Contains(string key)
        => Variables.ContainsKey(key);

    /// <summary>How many variables exist in this collection.</summary>
    [PublicAPI]
    public int Length => Variables.Count;

    /// <summary>Abstract override ToString() method to prepare the instance for a body.</summary>
    /// <returns>The prepared string for an HTTP body.</returns>
    public abstract override string ToString();
    
    /// <summary>Verification method for KeyValuePairs.</summary>
    /// <param name="key">The key of the collection item.</param>
    /// <param name="value">The value of the collection item.</param>
    /// <returns>true if the pair is valid, otherwise false.</returns>
    /// <remarks>This method should not be exposed.</remarks>
    protected virtual bool Verify(string key, string value)
        => true;
    
    /// <summary>Runs the ToString() method of the specified instance.</summary>
    /// <param name="instance">The instance to run the method on.</param>
    /// <returns>The result of the ToString() call in the instance.</returns>
    public static explicit operator string(BaseCollection instance)
        => instance.ToString();

    /// <summary>Key indexer for a collection.</summary>
    /// <param name="key">The key assigned to the desired value.</param>
    /// <exception cref="ArgumentException">The value set is null.</exception>
    /// <exception cref="ArgumentException">The value set is null.</exception>
    [DisallowNull]
    public string? this[string key]
    {
        get => Contains(key) ? Variables[key] : null;
        [NotNull] set
        {
            if (value is null)
                throw new ArgumentException("The value set cannot be null.", nameof(value));
            
            if (!Verify(key, value))
                throw new FormatException("key or value contain invalid format.");
            
            Variables[key] = value;
        }
    }
    
    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() 
        => Variables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}