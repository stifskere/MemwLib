using System.Text.RegularExpressions;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Identifiers;

internal class MixedIdentifier : IRequestIdentifier, IEquatable<string>
{
    private readonly object? _prefix;

    private readonly object _postfix = default!;
    
    public RequestMethodType RequestMethod { get; init; }
    
    public object? Prefix
    {
        get => _prefix;
        init
        {
            if (value is null)
            {
                _prefix = null;
                return;
            }
            
            if (value is Regex regexValue)
                value = new Regex($"^{regexValue}");
            
            SetFix(ref _prefix, value);
        } 
    }
    
    public required object Postfix
    {
        get => _postfix;
        init
        {
            if (value is Regex regexValue)
                value = new Regex($"{regexValue}$");
            
            SetFix(ref _postfix!, value);
        } 
    }

    private static void SetFix(ref object? field, object? value)
    {
        if (value?.GetType() != typeof(string) && value?.GetType() != typeof(Regex))
            throw new Exception();

        field = value;
    }
    
    public HttpRegexGroupCollection GetRegexGroups(string path)
    {
        HttpRegexGroupCollection collection = new();

        if (_prefix is Regex regexPrefix)
            collection.Add(regexPrefix.Match(path).Groups);
                
        if (_postfix is Regex regexPostfix)
            collection.Add(regexPostfix.Match(path).Groups);

        return collection;
    }

    public bool Equals(string? other)
    {
        if (other is null)
            return false;
        
        if (_prefix is Regex regexPrefix)
        {
            Match match = regexPrefix.Match(other);
            
            if (!match.Success)
                return false;

            other = other[match.Length..];
        } 
        else if (_prefix is string stringPrefix)
        {
            if (!other.StartsWith(stringPrefix))
                return false;

            other = other[stringPrefix.Length..];
        }

        if (_postfix is Regex regexPostfix)
        {
            Match match = regexPostfix.Match(other);

            if (!match.Success)
                return false;

            other = other[..^match.Length];
        } 
        else if (_postfix is string stringPostfix)
        {
            if (!other.EndsWith(stringPostfix))
                return false;

            other = other[..^stringPostfix.Length];
        }
        
        return string.IsNullOrEmpty(other);
    }
}