using System.Diagnostics;
using JetBrains.Annotations;

namespace MemwLib.Strings;

/// <summary>Class for adding colors to an string.</summary>
/// <remarks>This class is implicitly caster to string and vice versa.</remarks>
[PublicAPI]
public sealed class ColoredString : StringConverter
{
    private List<Range> _colorRanges = new();
    
    // TODO: implement
    /// <summary>Returns the length of the internal handle without the ansi color escape sequences.</summary>
    public int NormalizedLength { get; }
    
    // TODO: implement
    /// <summary>Returns the length of the internal handle with the ansi color escape sequences.</summary>
    public int ColoredLength => Handle.Length;
    
    /// <summary>Construct a ColoredString class from a basic string.</summary>
    /// <param name="handle">The string to construct from.</param>
    public ColoredString(string handle) : base(handle) { }

    private void MapColorRanges()
    {
        
    }
}