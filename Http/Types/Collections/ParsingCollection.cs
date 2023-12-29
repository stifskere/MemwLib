
namespace MemwLib.Http.Types.Collections;

/// <summary>
/// Abstract class for collections that need to be parsed,
/// contains common fields between all the collection types.
/// </summary>
/// <example>
/// Headers in HTTP requests/responses need to be
/// parsed from "Key: value" to an actual collection
/// </example>
public abstract class ParsingCollection : BaseIsolatedCollection<string, string?>
{
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
    public static explicit operator string(ParsingCollection instance)
        => instance.ToString();

    /// <summary>Key indexer for a collection.</summary>
    /// <param name="key">The key assigned to the desired value.</param>
    /// <exception cref="ArgumentException">The value set is null.</exception>
    public override string? this[string key]
    {
        get => Contains(key) ? Variables[key] : null;
        set
        {
            if (value is null)
                throw new ArgumentException("The value set cannot be null.", nameof(value));
            
            if (!Verify(key, value))
                throw new FormatException("key or value contain invalid format.");
            
            Variables[key] = value;
        }
    }
}