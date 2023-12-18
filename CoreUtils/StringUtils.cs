namespace MemwLib.CoreUtils;

internal static class StringUtils
{
    public static string TrimStart(this string instance, string toTrim)
    {
        while (!instance.StartsWith(toTrim)) 
            instance = instance[toTrim.Length..];

        return instance;
    }
}