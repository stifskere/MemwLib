using System.Collections;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Data;

public sealed partial class EnvContext : IEnumerable<KeyValuePair<string, string>>
{
    private readonly Dictionary<string, string> _variables = new();
    private readonly ImmutableDictionary<string, string> _env = Environment.GetEnvironmentVariables()
        .Cast<DictionaryEntry>()
        .ToImmutableDictionary(kvp => (string)kvp.Key, kvp => (string)kvp.Value!);
    
    [PublicAPI]
    public int Length => _variables.Count;
    
    [PublicAPI]
    public string this[string key] => _variables[key];

    public EnvContext(Stream data, bool closeOnFinish = false) : this(data, 0, (int)data.Length, closeOnFinish) {}
    
    public EnvContext(Stream data, int offset, int  length, bool closeOnFinish = false)
    {
        byte[] buffer = new byte[length];
        int len = data.Read(buffer, offset, length);
        FillFromData(Encoding.ASCII.GetString(buffer, 0, len));
        
        if (closeOnFinish)
            data.Close();
    }
    
    public EnvContext(string data)
        => FillFromData(data);

    private void FillFromData(string data)
    {
        foreach (Match match in EntryRegex().Matches(data))
        {
            string key = match.Groups["key"].Value;
            string value = match.Groups["value"].Value;

            if (value.StartsWith('\x22'))
                foreach (Match variable in SystemVariableRegex().Matches(value))
                    value = value.Replace(variable.Value, _env[variable.Groups["name"].Value]);

            _variables[key] = value.Trim('"', '\'', '\n');
        }
    }
    
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _variables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [PublicAPI]
    public bool Contains(string key)
        => _variables.ContainsKey(key);

    [GeneratedRegex(@"(?'key'[a-zA-Z_]+[a-zA-Z0-9_]*) *= *(?'value'(?(?=(?:\x22{3,}|\'{3,}))(?:\x22{3,}|\'{3,})(?:\n|.)+?(?:\x22{3,}|\'{3,})|(?(?=(?:\x22|\'){1,2})(?:\x22|\'){1}[^\x22'\n]+(?:\x22|\'){1}|[^\x22'\n]+)))")]
    private static partial Regex EntryRegex();

    [GeneratedRegex(@"\$\{(?'name'[a-zA-Z_][a-zA-Z0-9_]*)\}")]
    private static partial Regex SystemVariableRegex();
}