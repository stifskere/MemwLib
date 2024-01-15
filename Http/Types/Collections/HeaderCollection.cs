using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

/// <summary>Collection implementation for HTTP headers.</summary>
/// <remarks>The constructor for this collection is internal.</remarks>
[PublicAPI]
public sealed partial class HeaderCollection : ParsingCollection
{
    internal HeaderCollection() {}
    
    internal HeaderCollection(string collection)
    {
        MatchCollection matches;
        if (string.IsNullOrEmpty(collection) || (matches = HeaderVerification().Matches(collection)).Count == 0)
            return;
        
        foreach (Match header in matches)
        {
            string key = header.Groups["key"].Value,
                value = header.Groups["value"].Value;

            if (Contains(key))
                throw new ConstraintException("There is a duplicated key in this collection of headers.");

            this[key] = value;
        }
    }
    
    /// <inheritdoc cref="ParsingCollection.Verify"/>
    protected override bool Verify(string key, string value)
        => HeaderVerification().IsMatch($"{key}: {value}");

    /// <inheritdoc cref="ParsingCollection.ToString"/>
    public override string ToString()
        => Variables.Count == 0 ? string.Empty : Variables.Aggregate("", (old, iteration) => $"{old}{iteration.Key}: {iteration.Value}\r\n");
    
    [GeneratedRegex(@"(?'key'[a-zA-Z0-9\-_]+)\: (?'value'[\x20-\x7E]+)")]
    private static partial Regex HeaderVerification();
}