using System.Collections;
using System.Collections.Immutable;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.CoreUtils.Meta;
using MemwLib.Data.EnvironmentVariables.Attributes;

namespace MemwLib.Data.EnvironmentVariables;

/// <summary>Environment context is a Dictionary&lt;string, string&gt; encapsulated class to manage environment variables.</summary>
public sealed partial class EnvContext : IEnumerable<KeyValuePair<string, string>>
{
    private readonly Dictionary<string, string> _variables = new();
    
    private readonly ImmutableDictionary<string, string> _env = Environment.GetEnvironmentVariables()
        .Cast<DictionaryEntry>()
        .ToImmutableDictionary(kvp => (string)kvp.Key, kvp => (string)kvp.Value!);
    
    /// <summary>The amount of variables this context has.</summary>
    [PublicAPI]
    public int Count => _variables.Count;
    
    /// <summary>Value index operator.</summary>
    /// <param name="key">The key assigned to the value to get.</param>
    /// <returns>The value assigned to the key parameter.</returns>
    /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
    [PublicAPI]
    public string this[string key] => _variables[key];

    /// <summary>Creates a new instance of EnvContext<see href="https://hexdocs.pm/dotenvy/dotenv-file-format.html">, for accepted format guide see this</see>.</summary>
    /// <param name="useSystemEnv">Lets you decide whether to add the system environment variables or not.</param>
    [PublicAPI]
    public EnvContext(bool useSystemEnv = false)
    {
        if (!useSystemEnv) 
            return;
        
        foreach ((string key, string value) in _env)
            _variables.Add(key, value);
    }
    
    /// <summary>Adds variables from a stream of data reading the remaining length in the stream.</summary>
    /// <param name="data">Stream to read from.</param>
    /// <param name="closeOnFinish">Whether to close the stream after finished reading.</param>
    /// <exception cref="ArgumentException">The sum of offset and count is larger than the stream length.</exception>
    /// <exception cref="IOException">An I/O exception occurred in the underlying device.</exception>
    /// <exception cref="ConstraintException">There is a conflicting key between the data parameter and the instance.</exception>
    /// <exception cref="FormatException">The data is not well formatted <see href="https://hexdocs.pm/dotenvy/dotenv-file-format.html">, for environment variables</see>.</exception>
    [UsedImplicitly]
    public EnvContext AddVariablesFrom(Stream data, bool closeOnFinish = false)
        => AddVariablesFrom(data, data.Length, closeOnFinish);
    
    /// <summary>Adds variables from a stream of data till the specified length.</summary>
    /// <param name="data">Stream to read from.</param>
    /// <param name="length">The length to read from the stream</param>
    /// <param name="closeOnFinish">Whether to close the stream after finished reading.</param>
    /// <exception cref="ArgumentException">The sum of offset and count is larger than the stream length.</exception>
    /// <exception cref="IOException">An I/O exception occurred in the underlying device.</exception>
    /// <exception cref="ConstraintException">There is a conflicting key between the data parameter and the instance.</exception>
    /// <exception cref="FormatException">The data is not well formatted <see href="https://hexdocs.pm/dotenvy/dotenv-file-format.html">, for environment variables</see>.</exception>
    [UsedImplicitly]
    public EnvContext AddVariablesFrom(Stream data, long length, bool closeOnFinish = false)
    {
        byte[] buffer = new byte[length];
        int len = data.Read(buffer, 0, (int)length);
        FillFromData(Encoding.ASCII.GetString(buffer, 0, len));
        
        if (closeOnFinish)
            data.Close();

        return this;
    }
    
    /// <summary>Add environment variables from a formatted string.</summary>
    /// <param name="data">The string to parse from.</param>
    /// <exception cref="ConstraintException">There is a conflicting key between the data parameter and the instance.</exception>
    /// <exception cref="FormatException">The data is not well formatted for environment variables <see href="https://hexdocs.pm/dotenvy/dotenv-file-format.html"/></exception>
    [PublicAPI]
    public EnvContext AddVariablesFrom(string data)
    {
        FillFromData(data);
        return this;
    }
    
    /// <summary>Checks if there is a value assigned to a key.</summary>
    /// <param name="key">The key that should be assigned to the value.</param>
    /// <returns>true if the value exists, otherwise false.</returns>
    [PublicAPI, MustUseReturnValue]
    public bool Contains(string key)
        => _variables.ContainsKey(key);
    
    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _variables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Convert the current instance to a TInstance instance
    /// filling properties dynamically using reflections.
    /// </summary>
    /// <typeparam name="TInstance">The type that this instance must be converted to.</typeparam>
    /// <param name="caseSensitive">Define if the field naming is case sensitive or not.</param>
    /// <param name="flags">The meta binding flags to search for properties in the TInstance type.</param>
    /// <returns>A new instance of TInstance filled with the parameters found defined in the class.</returns>
    /// <remarks>
    /// This doesn't assume types as for standard,
    /// you will need to manually convert the string to the desired type
    /// </remarks>
    [PublicAPI, MustUseReturnValue]
    public TInstance ToType<TInstance>(bool caseSensitive, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) where TInstance : new()
    {
        TInstance instance = new();

        IDictionary<string, string> variablesToSeek =
            this.ToDictionary<KeyValuePair<string, string>, string, string>(
                caseSensitive 
                    ? p => p.Key
                    : p => p.Key.ToLower(),
                p => p.Value
            );
        
        MetaSearch.ProcessProperties<TInstance>(flags)
            .Exclude(properties => properties
                .Where(p => p.GetCustomAttribute<EnvironmentIgnoreAttribute>() is null)
            )
            .Do(property =>
            {
                EnvironmentVariableAttribute? altNameAttr = 
                    property.GetCustomAttribute<EnvironmentVariableAttribute>();

                string nameToSeek = caseSensitive 
                    ? altNameAttr?.Name ?? property.Name
                    : altNameAttr?.Name.ToLower() ?? property.Name.ToLower();
                
                if (variablesToSeek.TryGetValue(nameToSeek, out string? propertyValue))
                    property.SetValue(instance, propertyValue);
            });

        return instance;
    }
    
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
    
    [GeneratedRegex(@"(?'key'[a-zA-Z_]+[a-zA-Z0-9_]*) *= *(?'value'(?(?=(?:\x22{3,}|\'{3,}))(?:\x22{3,}|\'{3,})(?:\n|.)+?(?:\x22{3,}|\'{3,})|(?(?=(?:\x22|\'){1,2})(?:\x22|\'){1}[^\x22'\n]+(?:\x22|\'){1}|[^\x22'\n]+)))")]
    private static partial Regex EntryRegex();

    [GeneratedRegex(@"\$\{(?'name'[a-zA-Z_][a-zA-Z0-9_]*)\}")]
    private static partial Regex SystemVariableRegex();
}