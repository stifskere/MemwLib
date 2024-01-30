namespace MemwLib.Data.EnvironmentVariables.Attributes;

/// <summary>Will mark the current property as ignored by the EnvContext type converter.</summary>
[AttributeUsage(AttributeTargets.Property)]
public class EnvironmentIgnoreAttribute : Attribute { }