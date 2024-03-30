using JetBrains.Annotations;

namespace MemwLib.Data.EnvironmentVariables.Attributes;

/// <summary>Define an alternative name for EnvContext type converter.</summary>
/// <remarks>You most likely want to use this if you are following each language naming conventions.</remarks>
[AttributeUsage(AttributeTargets.Property), PublicAPI]
public class EnvironmentVariableAttribute : Attribute
{
    internal string Name { get; }
    
    /// <summary>Constructor to define the alternative name for the environment variable.</summary>
    /// <param name="name">The variable name.</param>
    public EnvironmentVariableAttribute(string name)
    {
        Name = name;
    }
}