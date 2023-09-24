using System.Collections;
using JetBrains.Annotations;
using DisallowNull = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;

namespace MemwLib.Http.Types.Collections;

public abstract class BaseCollection : IEnumerable<KeyValuePair<string, string>>
{
    protected readonly Dictionary<string, string> Variables = new();

    [PublicAPI]
    public bool Contains(string key)
        => Variables.ContainsKey(key);

    [PublicAPI]
    public int Length => Variables.Count;

    public abstract override string ToString();
    
    protected virtual bool Verify(string key, string value)
        => true;
    
    public static explicit operator string(BaseCollection instance)
        => instance.ToString();

    [DisallowNull]
    public string? this[string key]
    {
        get => Contains(key) ? Variables[key] : null;
        [NotNull] set
        {
            if (value is null)
                throw new ArgumentException("The value set cannot be null.", nameof(value));
            
            if (!Verify(key, value))
                throw new ArgumentException("key or value contain invalid format.", nameof(value));
            
            Variables[key] = value;
        }
    }
    
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() 
        => Variables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}