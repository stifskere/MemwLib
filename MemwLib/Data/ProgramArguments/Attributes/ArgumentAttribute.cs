using JetBrains.Annotations;

namespace MemwLib.Data.ProgramArguments.Attributes;

/// <summary>Modifier to change long and short name for an argument.</summary>
[AttributeUsage(AttributeTargets.Property), PublicAPI, MeansImplicitUse]
public sealed class ArgumentAttribute : Attribute
{
    internal readonly string ShortName;
    internal readonly string? LongName;
    
    /// <summary>Constructor for argument modifier.</summary>
    /// <param name="shortName">The short name for an argument.</param>
    /// <param name="longName">The long name for an argument.</param>
    public ArgumentAttribute(string shortName, string? longName)
    {
        ShortName = shortName;
        LongName = longName;
    }
}