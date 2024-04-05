
namespace MemwLib.CoreUtils.Collections;

/// <summary>
/// Abstract class for collections that need to be parsed,
/// contains common fields between all the collection types.
/// </summary>
/// <example>
/// Parameters in HTTP requests/responses need to be
/// parsed from "?key=value" to an actual collection,
/// in this case being a single key-value map.
/// </example>
public abstract class ParsingCollection : BaseIsolatedMap<string, string>
{
    /// <summary>Abstract override ToString() method to prepare the instance for a body.</summary>
    /// <returns>The prepared string for the target.</returns>
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
    /// <param name="key">
    /// The key assigned to the desired value,
    /// if null the key and value will be removed.
    /// </param>
    public new string? this[string key]
    {
        get => Contains(key) ? Variables[key] : null;
        set
        {
            if (value is not null && !Verify(key, value))
                throw new FormatException("key or value contain invalid format.");
            
            base[key] = value;
        }
    }
}