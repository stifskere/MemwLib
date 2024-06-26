using JetBrains.Annotations;
using MemwLib.CoreUtils.Collections;

namespace MemwLib.Http.Types.Collections;

/// <summary>A collection implementation for session parameters passed from middleware.</summary>
/// <remarks>The constructor for this collection is internal.</remarks>
[PublicAPI]
public sealed class SessionParameterCollection : BaseIsolatedMap<string, object>
{
    internal SessionParameterCollection() {}
    
    internal SessionParameterCollection(SessionParameterCollection collection) : base(collection) { }

    /// <inheritdoc cref="BaseIsolatedMap{TKey,TValue}.Get(TKey)"/>
    /// <typeparam name="TValue">The type of the value that is referenced by the key.</typeparam>
    public TValue? Get<TValue>(string key)
    {
        return (TValue?)base.Get(key);
    }
}