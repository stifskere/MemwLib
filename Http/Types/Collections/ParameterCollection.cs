using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

[PublicAPI]
public sealed partial class ParameterCollection : BaseCollection
{
    public ParameterCollection() {}

    public ParameterCollection(string collection)
    {
        if (string.IsNullOrEmpty(collection))
            return;
        
        if (collection.StartsWith('?'))
            collection = collection[1..];
        
        foreach (string parameter in collection.Split('&'))
        {
            string[] splitParameter = parameter.Split('=');
            
            if (splitParameter.Length != 2)
                throw new ArgumentException("Passed parameter collection contains invalid parameter");
            
            this[splitParameter[0]] = splitParameter[1];
        }
    }
    
    protected override bool Verify(string key, string value) 
        => ParameterVerification().IsMatch($"{key}={value}");

    public override string ToString() 
        => Variables.Count == 0 ? string.Empty : Variables.Aggregate("", (old, iteration) => $"{old}{iteration.Key}={iteration.Value}&")[..^1];
    
    [GeneratedRegex(@"^[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+=[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+$", RegexOptions.Singleline)]
    private static partial Regex ParameterVerification();
}