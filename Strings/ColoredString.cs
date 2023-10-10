using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Colors;

namespace MemwLib.Strings;

/// <summary>Static class to hold ANSI colored strings extension methods.</summary>
public static partial class ColoredString
{
#if DEBUG
    
    /// <summary>
    /// Adds an ANSI color modifier for foreground at the start and end of the specified
    /// range and moves the last conflicting modifier to the end of the range.
    /// </summary>
    /// <param name="handle">The string to add color to.</param>
    /// <param name="color">The color to set as an RGB24 instance.</param>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <returns>The colored string.</returns>
    /// <exception cref="NotImplementedException">Method is not implemented, will always throw.</exception>
    /// <remarks>
    /// Beware of the added modifiers, might alter any algorithm
    /// regarding string length, use only for output.
    /// </remarks>
    [PublicAPI]
    public static string SetForeground(this string handle, int start, int end, Rgb24 color)
        => handle.SetForeground(start..end, color);

    /// <summary>
    /// Adds an ANSI color modifier for foreground at the start and end of the specified
    /// range and moves the last conflicting modifier to the end of the range.
    /// </summary>
    /// <param name="handle">The string to add color to.</param>
    /// <param name="color">The color to set as an RGB24 instance.</param>
    /// <param name="range">The range where the color will be set.</param>
    /// <returns>The colored string.</returns>
    /// <exception cref="NotImplementedException">Method is not implemented, will always throw.</exception>
    /// <remarks>
    /// Beware of the added modifiers, might alter any algorithm
    /// regarding string length, use only for output.
    /// </remarks>
    [PublicAPI]
    public static string SetForeground(this string handle, Range range, Rgb24 color) 
        => handle.InsertHandle(range, color, ColorType.Foreground);
    
#endif
    
    /// <summary>
    /// Inserts ANSI foreground color at the start and end
    /// of the string, and removes all of ANSI modifiers in the range.
    /// </summary>
    /// <param name="handle">The string to add color to.</param>
    /// <param name="color">The color to set as an RGB24 instance.</param>
    /// <returns>The colored string.</returns>
    /// <remarks>
    /// Beware of the added modifiers, might alter any algorithm
    /// regarding string length, use only for output.
    /// </remarks>
    [PublicAPI]
    public static string SetForeground(this string handle, Rgb24 color)
        => handle.InsertHandle(color, ColorType.Foreground);
    
#if DEBUG
    
    /// <summary>
    /// Adds an ANSI color modifier for background at the start and end of the specified
    /// range and moves the last conflicting modifier to the end of the range.
    /// </summary>
    /// <inheritdoc cref="SetForeground(string, int, int, MemwLib.Colors.Rgb24)"/>
    [PublicAPI]
    public static string SetBackground(this string handle, int start, int end, Rgb24 color)
        => handle.SetBackground(start..end, color);
    
    /// <summary>
    /// Adds an ANSI color modifier for background at the start and end of the specified
    /// range and moves the last conflicting modifier to the end of the range.
    /// </summary>
    /// <inheritdoc cref="SetForeground(string, Range, MemwLib.Colors.Rgb24)"/>
    [PublicAPI]
    public static string SetBackground(this string handle, Range range, Rgb24 color) 
        => handle.InsertHandle(range, color, ColorType.Background);
    
#endif
    
    /// <summary>
    /// Inserts ANSI background color at the start and end
    /// of the string, and removes all of ANSI modifiers in the range.
    /// </summary>
    /// <inheritdoc cref="SetForeground(string, MemwLib.Colors.Rgb24)"/>
    [PublicAPI]
    public static string SetBackground(this string handle, Rgb24 color)
        => handle.InsertHandle(color, ColorType.Background);
    
    private static string InsertHandle(this string handle, Rgb24 color, ColorType colorType)
    {
        int rest = 0;
        foreach (Match match in Prefix().Matches(handle).Concat(Postfix().Matches(handle)))
        {
            int parsedType = int.Parse(match.Groups[0].Value);
                
            if ((int)colorType != parsedType && (int)colorType != parsedType + 1) 
                continue;
                
            handle = handle[..(match.Index - rest)] + handle[(match.Index + (match.Length - 1) - rest)..];
            rest += match.Length;
        }
        
        return $"\x1b[{(int)colorType};2;{color.R:D3};{color.G:D3};{color.B:D3}m{handle}\x1b[{(int)colorType + 1}m";
    }

    #if DEBUG
    // rewrite only using get real index and removing all the conflicts in between
    // ReSharper disable UnusedParameter.Local
    private static string InsertHandle(this string handle, Range range, Rgb24 color, ColorType colorType)
    {
        throw new NotImplementedException("Ranges are not yet implemented.");
        
/*
        if (range.Start.IsFromEnd)
            throw new ArgumentException("From end start values are not supported due to complexity.", nameof(range));

        if (range.Start.Value > handle.Length || range.End.Value > handle.Length)
            throw new ArgumentException("None of the range indexes can be bigger than the target string.", nameof(range));
        
        if (range.End.IsFromEnd)
            range = range.Start..(handle.Length - range.End.Value);

        range = GetRealIndex(range.Start.Value)..GetRealIndex(range.End.Value);
        
        if (GetConflicting(range.Start) is { } startConflict)
            range = (startConflict.EndIndex + 1)..range.End;
        
        string insertPrefix = $"\x1b[{(int)colorType};2;{color.R:D3};{color.G:D3};{color.B:D3}m";
        string insertPostfix = $"\x1b[{(int)colorType + 1}m";
        handle = handle.Insert(range.Start.Value, insertPrefix);
        range = GetRealIndex(range.Start.Value)..(range.End.Value + (insertPrefix.Length - 1));
        
        // this inserts ending at the start for some reason.

        if (range.Start.Value > range.End.Value)
            range = range.End..range.Start;
        
        if (GetConflicting(range.End) is not { } endConflict)
            return handle.Insert(range.End.Value, insertPostfix);
        
        if (endConflict.Type == FixType.Postfix && int.Parse(endConflict.Match.Groups[0].Value) - 1 == (int)colorType) 
            return handle;
            
        range = range.Start..endConflict.StartIndex;
        return handle.Insert(range.End.Value, insertPostfix);
*/

        /*Conflict? GetConflicting(Index index)
        {
            return FindConflict(Prefix().Matches(handle), FixType.Prefix) 
                   ?? FindConflict(Postfix().Matches(handle), FixType.Postfix);
            
            Conflict? FindConflict(MatchCollection matches, FixType type)
            {
                foreach (Match match in matches)
                    if (match.Index < index.Value && match.Index + match.Length < index.Value)
                        return new Conflict(match.Index, match.Index + match.Length, type, match);
                return null;
            }            
        }
        
        int GetRealIndex(int index)
        {
            bool isInFix = false;
            for (int i = 0; i < handle.Length; i++)
            {
                if (handle[i] == '\x1b')
                    isInFix = true;
                else if (isInFix && handle[i] == 'm')
                    isInFix = false;
                else if (!isInFix)
                {
                    if (index == 0)
                        return i;
                    index--;
                }
            }
            return handle.Length;
        }*/
    }

    /*private struct Conflict
    {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public FixType Type { get; }
        public Match Match { get; }

        public Conflict(int startIndex, int endIndex, FixType type, Match match)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Type = type;
            Match = match;
        }
    }*/
    
    // private enum FixType
    // {
    //     Prefix,
    //     Postfix
    // }
#endif
    
    private enum ColorType
    {
        Background = 48,
        Foreground = 38
    }
    
    [GeneratedRegex(@"\x1b\[(48|38);2(?:;\d{1,3}){3}")]
    private static partial Regex Prefix();
    
    [GeneratedRegex(@"\x1b\[(?:49|39)m")]
    private static partial Regex Postfix();
}