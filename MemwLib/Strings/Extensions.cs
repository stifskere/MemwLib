using JetBrains.Annotations;

namespace MemwLib.Strings;

/// <summary>String extension methods.</summary>
[PublicAPI]
public static class Extensions
{
    /// <summary>Separates a camel cased or pascal cased string with spaces.</summary>
    /// <param name="instance">the camel or pascal cased string.</param>
    /// <returns>The separated string result from the conversion.</returns>
    public static string Separate(this string instance)
    {
        string result = string.Empty;

        for (int i = 0; i < instance.Length; i++)
        {
            if (i != 0 && char.IsUpper(instance[i]))
                result += ' ';

            result += instance[i];
        }

        return result;
    }
}