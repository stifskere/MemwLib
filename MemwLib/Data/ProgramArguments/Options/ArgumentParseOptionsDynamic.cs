using JetBrains.Annotations;
using MemwLib.Data.ProgramArguments.Collections;

namespace MemwLib.Data.ProgramArguments.Options;

/// <summary>Options for command line argument parsing for dynamic output.</summary>                                                     
[PublicAPI]
public sealed class ArgumentParseOptionsDynamic : ArgumentParseOptions
{
    private ArgumentAliases _aliases = default!;
    
    /// <summary>
    /// Aliases for dynamic arguments, short argument names will
    /// search for its long pair and won't be set if not found.
    /// </summary>
    public ArgumentAliases Aliases
    {
        get => _aliases;
        set
        {
            ArgumentAliases lowerAliases = new();
            
            if (!CaseSensitive)
                foreach (var (name, alias) in value)
                    lowerAliases.AddAlias(name.ToLower(), alias.ToLower());

            _aliases = CaseSensitive ? value : lowerAliases;
        } 
    }
    
    /// <summary>
    /// Whether to assume types based on string
    /// comparison like true or false being a boolean
    /// </summary>
    public bool AssumeTypes { get; set; }
    
    /// <summary>
    /// Overwrite a duplicate key if true,
    /// otherwise throw ConstraintException
    /// </summary>
    public bool OverwriteDuplicates { get; set; }
}
