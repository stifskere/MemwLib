using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace MemwLib.Strings;

/// <summary>Handler to convert from char *, char[], string to each.</summary>
/// <remarks>This converter can only be instantiated from cast operators.</remarks>
[PublicAPI]
public class StringConverter
{
    /// <summary>The internal string handle for this converter.</summary>
    protected string Handle;

    /// <summary>Return the length of the underlying handle in this string converter.</summary>
    public int Length => Handle.Length;

    internal StringConverter(string handle)
    {
        Handle = handle;
    }
    
#pragma warning disable CS1591
    public static implicit operator StringConverter(string right)
        => new(right);

    public static unsafe implicit operator StringConverter(char* right)
    {
        string result = string.Empty;

        for (; *right != '\0'; right++)
            result += *right;

        return new StringConverter(result);
    }

    public static implicit operator StringConverter(char[] right) 
        => new(right.Aggregate(string.Empty, (current, t) => current + t));

    public static implicit operator string(StringConverter right) 
        => right.Handle;
    
    public static unsafe implicit operator char *(StringConverter right)
    {
        char* result = (char*)Marshal.AllocHGlobal(right.Handle.Length);
        char* cpy = result;

        for (int i = 0; i < right.Handle.Length; i++, cpy++)
            *cpy = right.Handle[i];

        *cpy = '\0';

        return result;
    }

    public static implicit operator char[](StringConverter right)
    {
        char[] result = new char[right.Handle.Length];

        for (int i = 0; i < right.Handle.Length; i++)
            result[i] = right.Handle[i];

        return result;
    }
#pragma warning restore CS1591
    
    /// <inheritdoc cref="string.this"/>
    public char this[int index]
    {
        get => Handle[index];
        set
        {
            char[] chars = Handle.ToCharArray();
            chars[index] = value;
            Handle = new string(chars);
        }
    }
}
