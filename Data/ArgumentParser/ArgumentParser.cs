using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using JetBrains.Annotations;
using MemwLib.CoreUtils.Meta;
using MemwLib.Data.ArgumentParser.Attributes;
using MemwLib.Data.ArgumentParser.Exceptions;
using MemwLib.Data.ArgumentParser.Options;

namespace MemwLib.Data.ArgumentParser;

/// <summary>Main class for console line arguments parser utilities.</summary>
[PublicAPI]
public static class ArgumentParser
{
    /// <summary>Parse arguments from a console line argument formatted string[].</summary>
    /// <param name="options">Configuration for the argument parsing.</param>
    /// <typeparam name="TParsed">The resulting type to return the parsed arguments as.</typeparam>
    /// <returns>The parsed arguments as a TParsed instance.</returns>
    /// <remarks><para>- Only the properties will be counted as valid fields for argument stubs.</para></remarks>
    /// <remarks><para>- Types that do not implement IParsable won't count as valid arguments, thus will be ignored.</para></remarks>
    public static TParsed Parse<TParsed>(ArgumentParseOptionsTyped? options = null) where TParsed : class, new()
    {
        options ??= new ArgumentParseOptionsTyped
        {
            Arguments = Environment.GetCommandLineArgs()
        };
        
        TParsed instance = new();

        ArgumentTypeAttribute? argumentType = typeof(TParsed).GetCustomAttribute<ArgumentTypeAttribute>();
        
        ArgumentTypeTreat treat = argumentType?.Treat 
                                  ?? ArgumentTypeTreat.OnlyPublic;

        BindingFlags flags = BindingFlags.SetProperty 
                                   | BindingFlags.Instance 
                                   | BindingFlags.Public 
                                   | BindingFlags.FlattenHierarchy;

        if (treat is ArgumentTypeTreat.All or ArgumentTypeTreat.AllWithArgumentAttribute)
            flags |= BindingFlags.NonPublic;
        
        MetaSearch.ProcessProperties<TParsed>(flags)
            .Exclude(properties => properties
                .Where(p => p.PropertyType.GetInterface(typeof(IParsable<>).Name) is not null 
                            || (Nullable.GetUnderlyingType(p.PropertyType)?.IsEnum ?? p.PropertyType.IsEnum))
                .DistinctBy(t => t.Name)
            )
            .ExcludeIf(
                treat is ArgumentTypeTreat.AllWithArgumentAttribute or ArgumentTypeTreat.OnlyPublicWithArgumentAttribute,
                properties => properties
                    .Where(p => p.GetCustomAttribute<ArgumentAttribute>() is not null)
            )
            .DoIf(
                property => ShouldSetValue(property, treat), 
                property => ProcessIndividualTypedArgument(instance, property, options)
            );

        return instance;
        
        static bool ShouldSetValue(PropertyInfo property, ArgumentTypeTreat treat)
        {
            bool isPublicProperty = property.GetSetMethod() is not null;
            bool hasArgumentAttribute = property.GetCustomAttribute<ArgumentAttribute>() is not null;
        
            return treat == ArgumentTypeTreat.All
                   || (treat == ArgumentTypeTreat.OnlyPublic && isPublicProperty)
                   || (treat == ArgumentTypeTreat.AllWithArgumentAttribute && hasArgumentAttribute)
                   || (treat == ArgumentTypeTreat.OnlyPublicWithArgumentAttribute && isPublicProperty && hasArgumentAttribute);
        }
    }
    
    /// <summary>Parse from a console line argument formatted string[].</summary>
    /// <param name="options">Configuration for the argument parsing.</param>
    /// <returns>A System.Dynamic.ExpandoObject aka dynamic instance containing all of the passed arguments</returns>
    /// <exception cref="ConstraintException">Thrown when a user defined constraint is not met.</exception>
    /// <remarks><para>- Types are assumed based on the input string, for type safety it's recommended to use Parse&lt;TParsed&gt; instead</para></remarks>
    /// <remarks><para>- All of the properties are lowercased</para></remarks>
    public static ExpandoObject Parse(ArgumentParseOptionsDynamic? options = null)
    {
        options ??= new ArgumentParseOptionsDynamic
        {
            Arguments = Environment.GetCommandLineArgs()
        };

        IDictionary<string, object?> result = new ExpandoObject();

        for (int i = 0; i < options.Arguments.Length; i++)
        {
            if (!StartsWithAnyPrefix(options.Arguments[i]))
                continue;
            
            ProcessIndividualDynamicArgument(
                options.Arguments[i], 
                options.Arguments.Length - 1 == i || StartsWithAnyPrefix(options.Arguments[i + 1]) ? null : options.Arguments[i + 1],
                options,
                ref result
                );
        }

        return (ExpandoObject)result;

        bool StartsWithAnyPrefix(string current)
        {
            Debug.Assert(options is not null);   
            
            return current.StartsWith(options.LongPrefix) || current.StartsWith(options.ShortPrefix);
        }
    }

    private static void ProcessIndividualDynamicArgument(string key, string? next, ArgumentParseOptionsDynamic options, ref IDictionary<string, object?> expando)
    {
        string? keyToSet = null;
        object? valueToSet = next;

        if (!options.CaseSensitive)
            key = key.ToLower();
        
        if (!key.StartsWith(options.LongPrefix) && key.StartsWith(options.ShortPrefix))
            keyToSet = options.Aliases.FindName(key[options.ShortPrefix.Length..]) ?? key[options.ShortPrefix.Length..];
        
        keyToSet ??= key[options.LongPrefix.Length..];

        if (options.ShouldExplicitlyHaveValue && next is null)
            throw new ConstraintException("Arguments passed should explicitly have a value.");

        if (expando.ContainsKey(keyToSet))
        {
            if (options.OverwriteDuplicates)
                expando.Remove(keyToSet);
            else
                throw new ConstraintException($"Duplicate key found, {keyToSet} already exists.");
        }
                
        
        if (options.AssumeTypes && next is not null)
        {
            if (
                (options.CaseSensitive ? next : next.ToLower()) 
                == (options.CaseSensitive ? bool.TrueString : bool.TrueString.ToLower())
            )
                valueToSet = true;

            if (
                (options.CaseSensitive ? next : next.ToLower()) 
                == (options.CaseSensitive ? bool.FalseString : bool.FalseString.ToLower())
            )
                valueToSet = false;

            if (double.TryParse(next, out double parsedDouble))
                valueToSet = parsedDouble;

            if (
                (options.CaseSensitive ? next : next.ToLower())
                == "null"
            )
                valueToSet = null;
        } 
        else if (options.AssumeTypes && next is null)
        {
            valueToSet = true;
        }
        
        expando.Add(keyToSet, valueToSet);
    }
    
    private static void ProcessIndividualTypedArgument<TParsed>(TParsed instance, PropertyInfo property, ArgumentParseOptionsTyped options)
    {
        ArgumentAttribute? attribute = property.GetCustomAttribute<ArgumentAttribute>();
        
        string[] possibleNames = attribute is not null 
            ? new[] { $"{options.LongPrefix}{attribute.LongName ?? property.Name}", $"{options.ShortPrefix}{attribute.ShortName}" } 
            : new[] { $"{options.LongPrefix}{property.Name}" };
        
        if (!options.CaseSensitive)
        {
            possibleNames = possibleNames
                .Select(n => n.ToLower())
                .ToArray();

            options.Arguments = options.Arguments
                .Select(a => a.StartsWith(options.LongPrefix) || a.StartsWith(options.ShortPrefix) ? a.ToLower() : a)
                .ToArray();
        }
        
        int keyIndex = -1;
        foreach (string key in possibleNames)
        {
            keyIndex = Array.IndexOf(options.Arguments, key);
            
            if (keyIndex != -1)
                break;
        }

        if (!options.ShouldExplicitlyHaveValue && keyIndex == -1 && property.PropertyType == typeof(bool))
        {
            property.SetValue(instance, false);
            return;
        }

        if (keyIndex == -1 && Nullable.GetUnderlyingType(property.PropertyType) is null)
            throw new ArgumentNonOptionalException(property);
        
        if (keyIndex == -1)
        {
            property.SetValue(instance, null);
            return;
        }

        string value = options.Arguments.Length != keyIndex + 1 
            ? options.Arguments[keyIndex + 1]
            : string.Empty;
        
        if (!options.ShouldExplicitlyHaveValue && value.StartsWith(options.LongPrefix) || value.StartsWith(options.ShortPrefix) 
            || string.IsNullOrEmpty(value) && property.PropertyType == typeof(bool))
        {
            property.SetValue(instance, true);
            return;
        }

        try
        {
            property.SetValue(instance, TypeDescriptor.GetConverter(property.PropertyType).ConvertFromInvariantString(value));
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException)
        {
            throw new ConvertArgumentException(property, value, exception);
        }
    }
}