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
}