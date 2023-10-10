using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

//TODO: redo implementation to always verify.

/// <summary>Collection implementation for HTTP headers.</summary>
[PublicAPI]
public sealed partial class HeaderCollection : BaseCollection
{
    /// <summary>Constructor for empty collection.</summary>
    public HeaderCollection() {}

    /// <summary>Parses http formatted HTTP headers to a manageable collection.</summary>
    /// <param name="collection">The formatted HTTP headers collection.</param>
    public HeaderCollection(string collection)
    {
        if (string.IsNullOrEmpty(collection))
            return;
        
        foreach (string header in collection.Split("\r\n"))
        {
            string[] splitHeader = header.Split(": ");

            if (splitHeader.Length >= 2)
                this[splitHeader[0]] = string.Join("\r\n", splitHeader[1..]);
        }
    }
    
    /// <inheritdoc cref="BaseCollection.Verify"/>
    protected override bool Verify(string key, string value)
        => HeaderVerification().IsMatch($"{key}: {value}");

    /// <inheritdoc cref="BaseCollection.ToString"/>
    public override string ToString()
        => Variables.Count == 0 ? string.Empty : Variables.Aggregate("", (old, iteration) => $"{old}{iteration.Key}: {iteration.Value}\r\n");
    
    [GeneratedRegex(@"^[a-zA-Z0-9\-_]+\: [\x20-\x7E]+$")]
    private static partial Regex HeaderVerification();
}