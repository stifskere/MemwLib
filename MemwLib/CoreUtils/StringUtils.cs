using System.Text;

namespace MemwLib.CoreUtils;

internal static class StringUtils
{
    public static string CuTrimStart(this string instance, string toTrim = " ")
    {
        while (!instance.StartsWith(toTrim)) 
            instance = instance[toTrim.Length..];

        return instance;
    }

    public static string CuTrimEnd(this string instance, string toTrim = " ")
    {
        while (!instance.EndsWith(toTrim))
            instance = instance[..^toTrim.Length];

        return instance;
    }

    public static string CuTrim(this string instance, string toTrim = " ") 
        => instance
            .CuTrimStart(toTrim)
            .CuTrimEnd(toTrim);

    public static bool StartsWithAnyOf(this string instance, params char[] characters) 
        => characters.Contains(instance[0]);

    public static bool EndsWithAnyOf(this string instance, params char[] characters)
        => characters.Contains(instance[^1]);

    public static bool IsEnclosedWithSameOf(this string instance, params char[] characters)
        => instance.Length > 1 && instance.StartsWithAnyOf(characters) && instance.EndsWithAnyOf(instance[0]);

    public static bool IsEnclosedBy(this string instance, char start, char end)
        => instance.StartsWith(start) && instance.EndsWith(end);
    
    public static string Repeat(this string instance, int times)
    {
        string result = string.Empty;

        for (int i = 0; i < times; i++)
            result += instance;

        return result;
    }

    public static string GetRaw(this MemoryStream instance, int? length = null)
    {
        byte[] array = new byte[length ?? instance.Length];
        _ = instance.Read(array, 0, length ?? array.Length);
        return Encoding.ASCII.GetString(array);
    }
}