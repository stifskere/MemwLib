using System.Collections;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MemwLib.Data;

public sealed partial class EnvFile : IEnumerable<KeyValuePair<string, string>>
{
    private readonly Dictionary<string, string> _variables = new();
    
    [PublicAPI]
    public int Length => _variables.Count;
    
    [PublicAPI]
    public string this[string key] => _variables[key];
    
    public EnvFile(string path)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Close();
            return;
        }
        
        bool multiLine = false, interpolated = false;
        string cMultiKey = string.Empty, cMultiVal = string.Empty;
        foreach (string line in File.ReadLines(path))
        {
            Match currentEntry = EntryRegex().Match(line);

            if (!multiLine)
            {
                if (line.StartsWith('#') || !currentEntry.Success)
                    continue;

                string val = currentEntry.Groups["key"].Value.TrimStart();
                if (!(multiLine = val.StartsWith("\"\"\"") || val.StartsWith("'''")))
                {
                    if (currentEntry.Groups.ContainsKey("key"))
                    {
                        _variables[currentEntry.Groups["key"].Value]
                            = currentEntry.Groups["value"].Value;
                    }

                    continue;
                }
                
                cMultiKey = currentEntry.Groups["key"].Value;
                interpolated = cMultiKey.StartsWith("\"\"\"");
                cMultiVal += currentEntry.Groups["value"].Value;
            }

            if (cMultiVal.StartsWith(MultiLineFix()))
                cMultiVal = cMultiVal[3..];
            else
                cMultiVal += line;
            
            
            if (interpolated)
                foreach (Match variable in SystemVariableRegex().Matches(line))
                {
                    string? result = Environment.GetEnvironmentVariable(variable.Value[2..^1]);

                    if (result is not null)
                        cMultiVal = cMultiVal.Replace(variable.Value, result);
                }

            if (cMultiVal.Contains(MultiLineFix()))
            {
                cMultiVal = cMultiVal[..^3];
                multiLine = false;
                _variables[cMultiKey] = cMultiVal;
            }
        }

        return;

        string MultiLineFix() => interpolated ? "\"\"\"" : "'''";
    }
    
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _variables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [PublicAPI]
    public bool Contains(string key)
        => _variables.ContainsKey(key);

    [GeneratedRegex("(?'key'[a-zA-Z_]+[a-zA-Z0-9_]*)=(?'value'(?:\"\"\"|''')?.+)")]
    private static partial Regex EntryRegex();

    [GeneratedRegex(@"\$\{[a-zA-Z_]+[a-zA-Z0-9_]*\}")]
    private static partial Regex SystemVariableRegex();
}