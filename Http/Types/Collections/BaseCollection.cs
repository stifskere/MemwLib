using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

public abstract class BaseCollection
{
    protected Dictionary<string, string> Variables = new();

    [PublicAPI]
    public bool Contains(string key)
        => Variables.ContainsKey(key);
    
    public abstract override string ToString();

    protected virtual bool Verify(string key, string value)
        => true;
    
    public static explicit operator string(BaseCollection instance)
        => instance.ToString();

    public string this[string key]
    {
        get => Variables[key];
        set
        {
            if (!Verify(key, value))
                throw new ArgumentException("key or value contain invalid format.");
            
            Variables[key] = value;
        }
    }
}