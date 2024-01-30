using JetBrains.Annotations;

namespace MemwLib.Data.ProgramArguments.Attributes;

/// <summary>Define the behavior for how to treat an argument class.</summary>
[AttributeUsage(AttributeTargets.Class), PublicAPI, MeansImplicitUse]
public sealed class ArgumentTypeAttribute : Attribute
{
    internal readonly ArgumentTypeTreat Treat;
    
    /// <summary>Argument type constructor.</summary>
    /// <param name="treat">Tell the parser how to treat the class.</param>
    public ArgumentTypeAttribute(ArgumentTypeTreat treat)
    {
        Treat = treat;
    }
}

/// <summary>Behavior definitions for how to treat an argument class.</summary>
public enum ArgumentTypeTreat
{
    /// <summary>Will only parse all private and public properties but only with argument attribute.</summary>
    AllWithArgumentAttribute,
    /// <summary>Will only parse public properties but only with argument attribute.</summary>
    OnlyPublicWithArgumentAttribute,
    /// <summary>Will parse private and public properties even without the argument attribute.</summary>
    All,
    /// <summary>Will only parse public properties even without the argument attribute.</summary>
    OnlyPublic
}