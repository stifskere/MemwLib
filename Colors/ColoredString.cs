using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Colors.Types;

namespace MemwLib.Colors;

public static partial class ColoredString
{
    [PublicAPI]
    public static string SetForeground(this string handle, int first, int last, Color color)
        => handle.SetForeground(first..last, color);
    
    [PublicAPI]
    public static string SetForeground(this string handle, Range range, Color color) 
        => InsertHandle(handle, range, color, 38);

    [PublicAPI]
    public static string SetBackground(this string handle, int first, int last, Color color)
        => handle.SetBackground(first..last, color);

    [PublicAPI]
    public static string SetBackground(this string handle, Range range, Color color) 
        => InsertHandle(handle, range, color, 48);

    private static string InsertHandle(string handle, Range range, Color color, byte type)
    {
        string ansiPrefix = $"\x1b[{type:D3};2;{color.R:D3};{color.G:D3};{color.B:D3}m";
        string ansiPostfix = "\x1b[0m";
        
        for (int i = 0; i < handle.Length; i++)
        {
            
        }
    }

    [GeneratedRegex(@"(\x1b\[\d{3};2;\d{3};\d{3};\d{3}m)")]
    private static partial Regex AnsiPrefixRegex();
    
    [GeneratedRegex(@"(\x1b\[0m)")]
    private static partial Regex AnsiPostfixRegex();
}