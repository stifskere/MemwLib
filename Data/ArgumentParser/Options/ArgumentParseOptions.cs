using System.Data;
using JetBrains.Annotations;

namespace MemwLib.Data.ArgumentParser.Options;

// TODO: definitively change ShortPrefix for an string non auto property back.

/// <summary>Base options for command line argument parsing.</summary>
[PublicAPI]
public abstract class ArgumentParseOptions
{
    private string _longPrefix = "--";
    private string _shortPrefix = "-";
    private string[] _arguments = default!;
    
    /// <summary>The arguments to parse from.</summary>
    public required string[] Arguments
    {
        get => _arguments;
        set
        {
            if (!CaseSensitive)
                for (int i = 0; i < value.Length; i++)
                    value[i] = value[i].ToLower();

            _arguments = value;
        } 
    }

    /// <summary>The prefix for long argument keys.</summary>
    /// <example>--key value</example>
    public string LongPrefix
    {
        get => _longPrefix;
        set
        {
            if (value.Length <= _shortPrefix.Length)
                throw new ConstraintException("Long prefix length shouldn't be <= than short prefix length.");

            _longPrefix = CaseSensitive ? value : value.ToLower();
        } 
    }
    
    /// <summary>The prefix for short argument keys.</summary>
    /// <example>-k value</example>
    public string ShortPrefix
    {
        get => _shortPrefix;
        set
        {
            if (value.Length >= _longPrefix.Length)
                throw new ConstraintException("Short prefix length shouldn't be >= than long prefix length.");

            _shortPrefix = CaseSensitive ? value : value.ToLower();
        } 
    }

    /// <summary>Defines if the arguments should explicitly have a value and not infer behavior from type.</summary>
    /// <example>
    /// the option "--enable" could be a boolean,
    /// if the behavior is inferred from usage it will be true on present, whether the value is true or has no value.
    /// </example>
    public bool ShouldExplicitlyHaveValue { get; set; }

    /// <summary>Defines whether property lookup is case sensitive or not.</summary>
    public bool CaseSensitive { get; init; } = true;
}