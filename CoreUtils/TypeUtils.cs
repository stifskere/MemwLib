namespace MemwLib.CoreUtils;

internal static class TypeUtils
{
    public static object InferTypeFrom(string value)
    {
        throw new NotImplementedException();
    }

    public static bool TargetEnumHasFlags<TEnum>(TEnum target, TEnum flags) where TEnum : Enum
    {
        string[] convertedTarget = target.ToString().Replace(" ", "").Split(',');
        string[] convertedFlags = flags.ToString().Replace(" ", "").Split(',');

        return convertedFlags.Any(flag => convertedTarget.Contains(flag));
    }
}