using System.Reflection;
using System.Text.RegularExpressions;
using MemwLib.Http.Types.Content.Views.Exceptions;

namespace MemwLib.Http.Types.Content.Views;

// TODO: change how location works

public static class ViewHelper
{
    private static readonly IEnumerable<Assembly> Assemblies = AppDomain.CurrentDomain.GetAssemblies();
    private static readonly Dictionary<string, Type> TypeCache = new();
    
    public static Type? GetTypeByName(string name)
    {
        if (TypeCache.TryGetValue(name, out Type? cached))
            return cached;
        
        foreach (Assembly assembly in Assemblies)
        {
            if (assembly.GetTypes().FirstOrDefault(t => t.Name == name) is not { } type) 
                continue;
            
            TypeCache.Add(name, type);
            return type;
        }

        return null;
    }

    public static string[] DivideComplexExpression(string expression, Location location)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new EmptyExpressionException(location);

        List<string> sections = [];
        
        string current = string.Empty;
        
        bool inParentheses = false;
        bool inString = false;
        
        for (int i = 0; i < expression.Length; i++)
        {
            if (expression[i] == ' ' && !inString)
                continue;
            
            switch (expression[i])
            {
                case '"':
                    inString = !inString;
                    break;
                case '(' when !inString:
                    inParentheses = true;
                    break;
                case ')' when !inString:
                    inParentheses = false;
                    break;
            }

            if (char.IsLetterOrDigit(expression[i]) 
                || expression[i] is '(' or ')' or '"'
                || inString || inParentheses)
            {
                current += expression[i];
                continue;
            }

            if (i == expression.Length - 1)
                throw new InvalidExpressionException(
                    location.Align(i, expression), 
                    $"Invalid character for end of sequence {expression[i]}"
                );

            if ((expression[i] != ':' || expression[i + 1] != ':') 
                && (expression[i] != '-' || expression[i + 1] != '>')) 
                throw new InvalidExpressionException(
                    location.Align(i, expression),
                    $"Invalid character found {expression[i]}"
                );
            
            sections.Add(current);
            sections.Add($"{expression[i]}{expression[i + 1]}");
            current = "";
            i++;
        }

        sections.Add(current);
        
        if (inParentheses || inString)
            throw new InvalidExpressionException(location, "Unexpected end of sequence, missing \" or )");
        
        return sections.ToArray();
    }

    public static object? ProcessComplexExpression
        (string expression, Location location, Dictionary<string, object?> references)
    {
        expression = expression.Trim();

        if (expression == "null")
            return null;
        
        if (expression.StartsWith('"') && expression.EndsWith('"'))
            return expression[1..^1];

        if (int.TryParse(expression, null, out int asInt)) 
            return asInt;

        if (double.TryParse(expression, out double asDouble))
            return asDouble;

        if (bool.TryParse(expression, out bool asBool))
            return asBool;
        
        string[] divided = DivideComplexExpression(expression, location);

        object? followUp = references.TryGetValue(divided[0], out object? value) 
            ? value 
            : GetTypeByName(divided[0]);
        
        if (followUp is null)
            throw new SymbolNotFoundException(divided[0], location);

        for (int i = 2; i < divided.Length; i += 2)
        {
            location = location.Align(divided[i].Length + 2, expression);
            followUp = ProcessOne(divided[i], divided[i - 1] == "::");
        }

        return followUp;
        
        object? ProcessOne(string section, bool isStatic)
        {
            section = section.Trim();
            
            BindingFlags flags =
                (isStatic ? BindingFlags.Static : BindingFlags.Instance)
                | BindingFlags.Public;

            Match function 
                = Regex.Match(section, @"^(?<name>[A-Za-z]\w+)\((?<parameters>(?:[^()]|\([^()]*\))*)\)$");
            
            Type? followUpType = followUp as Type ?? followUp?.GetType();
            
            if (followUpType is null)
                throw new NullReferenceException(
                    $"Cannot access member of null, accessing {section}; at {location}"
                );
            
            if (function.Success)
            {
                string name = function.Groups["name"].Value;
                object?[] parameters = string.IsNullOrEmpty(function.Groups["parameters"].Value) 
                    ? Array.Empty<object?>()
                    : function
                        .Groups["parameters"]
                        .Value
                        .Split(',')
                        .Select(p => ProcessComplexExpression(p.Trim(), location, references))
                        .ToArray();

                if (name == "new")
                {
                    if (!isStatic || followUp is not Type)
                        throw new InvalidExpressionException(
                            location, 
                            !isStatic 
                                ? "Constructors must only be called statically."
                                : "Constructors can only be called on types."
                        );
                    
                    return Activator.CreateInstance(followUpType, parameters);
                }

                MethodInfo? method = followUpType.GetMethod(
                    name, 
                    flags, 
                    parameters.Select(p => p?.GetType() ?? typeof(Nullable<>)).ToArray()
                );

                if (method is null)
                    throw new SymbolNotFoundException(
                        name, 
                        location
                    );

                return method.Invoke(followUp, parameters);
            }

            if (followUpType.GetProperty(section, flags) is { } property)
                return property.GetValue(followUp);
            
            if (followUpType.GetField(section, flags) is { } field)
                return field.GetValue(followUp);

            throw new SymbolNotFoundException(section, location);
        }
    }

    public static string[] DivideArithmeticExpression(string expression)
    {
        throw new NotImplementedException();
    }
    
    public static object? ProcessArithmeticExpression(string expression)
    {
        throw new NotImplementedException();
    }
}