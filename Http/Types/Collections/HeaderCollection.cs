using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

/// <summary>Collection implementation for HTTP headers.</summary>
[PublicAPI]
public sealed partial class HeaderCollection : BaseCollection
{
    /// <summary>Constructor for empty collection.</summary>
    public HeaderCollection() {}

    /// <summary>Parses http formatted HTTP headers to a manageable collection.</summary>
    /// <param name="collection">The formatted HTTP headers collection.</param>
    /// <exception cref="FormatException">The header collection is empty or was not correctly formatted.</exception>
    /// <exception cref="ConstraintException">There is a duplicated key in the collection.</exception>
    public HeaderCollection(string collection)
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
    
    /// <inheritdoc cref="BaseCollection.Verify"/>
    protected override bool Verify(string key, string value)
        => HeaderVerification().IsMatch($"{key}: {value}");

    /// <inheritdoc cref="BaseCollection.ToString"/>
    public override string ToString()
        => Variables.Count == 0 ? string.Empty : Variables.Aggregate("", (old, iteration) => $"{old}{iteration.Key}: {iteration.Value}\r\n");
    
    [GeneratedRegex(@"(?'key'[a-zA-Z0-9\-_]+)\: (?'value'[\x20-\x7E]+)")]
    private static partial Regex HeaderVerification();
}