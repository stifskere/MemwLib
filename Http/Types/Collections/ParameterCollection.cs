using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

// TODO: redo implementation to always verify

/// <summary>Collection implementation for HTTP URI parameters.</summary>
[PublicAPI]
public sealed partial class ParameterCollection : BaseCollection
{
    /// <summary>Constructor for empty collection.</summary>
    public ParameterCollection() {}

    /// <summary>Parses formatted HTTP URI request parameters to a manageable collection.</summary>
    /// <param name="collection">The formatted HTTP URI request parameters.</param>
    /// <exception cref="FormatException">The parameters were not correctly formatted.</exception>
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
                throw new FormatException("Passed parameter collection contains invalid parameter");
            
            this[splitParameter[0]] = splitParameter[1];
        }
    }
    
    /// <inheritdoc cref="BaseCollection.Verify"/>
    protected override bool Verify(string key, string value) 
        => ParameterVerification().IsMatch($"{key}={value}");

    /// <inheritdoc cref="BaseCollection.ToString"/>
    public override string ToString() 
        => Variables.Count == 0 ? string.Empty : Variables.Aggregate("", (old, iteration) => $"{old}{iteration.Key}={iteration.Value}&")[..^1];
    
    [GeneratedRegex(@"^[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+=[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+$", RegexOptions.Singleline)]
    private static partial Regex ParameterVerification();
}