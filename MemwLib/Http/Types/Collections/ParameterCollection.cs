using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Types.Collections;

/// <summary>Collection implementation for HTTP URI parameters.</summary>
/// <remarks>The constructor for this collection is internal.</remarks>
[PublicAPI]
public sealed partial class ParameterCollection : ParsingCollection
{
    internal ParameterCollection() {}
    
    internal ParameterCollection(string collection)
    {
        MatchCollection matches;
        if (string.IsNullOrEmpty(collection) || (matches = ParameterVerification().Matches(collection)).Count == 0)
            return;
        
        foreach (Match parameter in matches)
        {
            string key = UriHelpers.DecodeUriComponent(parameter.Groups["key"].Value),
                value = UriHelpers.DecodeUriComponent(parameter.Groups["value"].Value);

            if (Contains(key))
                throw new ConstraintException("There is a duplicated key in this collection of parameters.");

            this[key] = value;
        }
    }
    
    /// <inheritdoc cref="ParsingCollection.Verify"/>
    protected override bool Verify(string key, string value) 
        => ParameterVerification().IsMatch($"{key}={value}");

    /// <inheritdoc cref="ParsingCollection.ToString"/>
    public override string ToString() 
        => Variables.Count == 0 ? string.Empty : Variables.Aggregate("", (old, iteration) 
            => $"{old}{UriHelpers.EncodeUriComponent(iteration.Key)}={UriHelpers.EncodeUriComponent(iteration.Value!)}&")[..^1];
    
    [GeneratedRegex(@"(?'key'[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+)=(?'value'[a-zA-Z0-9;,/\\:@+$\-_.!~*'()]+)", RegexOptions.Singleline)]
    private static partial Regex ParameterVerification();
}