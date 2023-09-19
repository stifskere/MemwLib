using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Colors.Types;

namespace MemwLib.Colors;

public static partial class ColoredString
{
    [PublicAPI]
    public static string SetForeground(this string handle, int first, int last, Rgb24 color)
        => handle.SetForeground(first..last, color);
    
    [PublicAPI]
    public static string SetForeground(this string handle, Range range, Rgb24 color) 
        => InsertHandle(handle, range, color, 38);

    [PublicAPI]
    public static string SetBackground(this string handle, int first, int last, Rgb24 color)
        => handle.SetBackground(first..last, color);

    [PublicAPI]
    public static string SetBackground(this string handle, Range range, Rgb24 color) 
        => InsertHandle(handle, range, color, 48);

    private static string InsertHandle(string handle, Range range, Rgb24 color, byte type)
    {
        return "";
        
        string ansiPrefix = $"\x1b[{type:D3};2;{color.R:D3};{color.G:D3};{color.B:D3}m";
        string ansiPostfix = $"\x1b[{type:D3};0m";
        range = new Range(GetRealIndex(range.Start.Value), GetRealIndex(range.Start.Value));
        
        

        int GetRealIndex(int index)
        {
            bool isInConverter = false;
            int i = 0;
            for (int x = 0; i < handle.Length; i++)
            {
                if (handle[i] == '\x1b')
                    isInConverter = true;

                if (isInConverter && handle[i] == 'm')
                    isInConverter = false;

                if (!isInConverter)
                    x++;

                if (index >= x)
                    break;
            }

            return i;
        }
    }

    [GeneratedRegex(@"\x1b\[\d{1,3};2;\d{1,3};\d{1,3};\d{1,3}m")]
    private static partial Regex AnsiPrefixRegex();
    
    [GeneratedRegex(@"\x1b\[{\d{1,3};0m")]
    private static partial Regex AnsiPostfixRegex();
}