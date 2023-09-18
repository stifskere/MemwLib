using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Collections;

[PublicAPI]
public sealed partial class HeaderCollection : BaseCollection
{
    public HeaderCollection() {}

    public HeaderCollection(string collection)
    {
        if (string.IsNullOrEmpty(collection))
            return;
        
        foreach (string header in collection.Split("\r\n"))
        {
            int splitIndex = header.IndexOf(':');

            if (splitIndex == -1)
                throw new ArgumentException("Passed header collection contains invalid header");
            
            string[] splitHeader = { header[..splitIndex], header[(splitIndex + 1)..].TrimStart() };

            this[splitHeader[0]] = splitHeader[1];
        }
    }
    
    protected override bool Verify(string key, string value)
        => HeaderVerification().IsMatch($"{key}: {value}");

    public override string ToString()
        => Variables.Aggregate("", (old, iteration) => $"{old}{iteration.Key}: {iteration.Value}\r\n");
    
    [GeneratedRegex(@"^[a-zA-Z0-9\-_]+\: [\x20-\x7E]+$")]
    private static partial Regex HeaderVerification();
}