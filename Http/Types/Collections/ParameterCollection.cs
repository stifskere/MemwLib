using System.Text.RegularExpressions;
using JetBrains.Annotations;
using System.Data;

namespace MemwLib.Http.Types.Collections;

/// <summary>Collection implementation for HTTP URI parameters.</summary>
[PublicAPI]
public sealed partial class ParameterCollection : BaseCollection
{
    /// <summary>Constructor for empty collection.</summary>
    public ParameterCollection() {}

    /// <summary>Parses formatted HTTP URI request parameters to a manageable collection.</summary>
    /// <param name="collection">The formatted HTTP URI request parameters.</param>
    /// <exception cref="FormatException">The parameter collection is empty or was not correctly formatted.</exception>
    /// <exception cref="ConstraintException">There is a duplicated key in the collection.</exception>
    public ParameterCollection(string collection)
    {
        MatchCollection matches;
        if (string.IsNullOrEmpty(collection) || (matches = ParameterVerification().Matches(collection)).Count == 0)
            throw new FormatException("The parameters were not correctly formatted.");
        
        foreach (Match parameter in matches)
        {
            string key = parameter.Groups["key"].Value,
                value = parameter.Groups["value"].Value;

            if (Contains(key))
                throw new ConstraintException("There is a duplicated key in this collection of parameters.");

            this[key] = value;
        }
    }
    
    /// <inheritdoc cref="BaseCollection.Verify"/>
    protected override bool Verify(string key, string value) 
        => ParameterVerification().IsMatch($"{key}={value}");

    /// <inheritdoc cref="BaseCollection.ToString"/>
    public override string ToString() 
        => Variables.Count == 0 ? string.Empty : Variables.Aggregate("", (old, iteration) => $"{old}{iteration.Key}={iteration.Value}&")[..^1];
    
    [GeneratedRegex(@"(?'key'[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+)=(?'value'[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+)", RegexOptions.Singleline)]
    private static partial Regex ParameterVerification();
}