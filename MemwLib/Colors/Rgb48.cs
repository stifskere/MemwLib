using JetBrains.Annotations;

namespace MemwLib.Colors;

/// <summary>48-bit RGB representation.</summary>
public class Rgb48
{
    /// <inheritdoc cref="Rgb24.R"/>
    [PublicAPI]
    public ushort R { get; set; }
    
    /// <inheritdoc cref="Rgb24.G"/>
    [PublicAPI]
    public ushort G { get; set; }
    
    /// <inheritdoc cref="Rgb24.B"/>
    [PublicAPI]
    public ushort B { get; set; }

    /// <inheritdoc cref="Rgb24(byte, byte, byte)"/>
    public Rgb48(ushort r, ushort g, ushort b)
    {
        R = r;
        G = g;
        B = b;
    }

    /// <summary>The unsigned integer value constructor.</summary>
    /// <param name="color">The decimal representation of the color.</param>
    /// <exception cref="ArgumentException">The color exceeds the 0xFFFFFFFFFFFF value.</exception>
    public Rgb48(ulong color) 
        : this((ushort)((color >> 32) & 0xFFFF), (ushort)((color >> 16) & 0xFFFF), (ushort)(color & 0xFFFF))
    {
        if (color > 0xFFFFFFFFFFFF)
            throw new ArgumentException("Color value exceeds maximum unsigned short value.", nameof(color));
    }
    
    /// <summary>Rgb64 cast constructor, constructs an instance of RGB48 from an instance of RGB64.</summary>
    /// <param name="color">The instance to cast from.</param>
    public Rgb48(Rgb64 color) : this((ulong)color >> 16) { }

    /// <summary>Obtain the color decimal value as an unsigned long integer instance.</summary>
    [PublicAPI]
    public virtual ulong AsUlong
        => (ulong)R << 32 | (ulong)G << 16 | B;

    
    /// <summary>Cast an instance of RGB48 to an instance of unsigned long integer.</summary>
    /// <param name="instance">The instance to cast.</param>
    /// <returns>The casted instance as unsigned long integer.</returns>
    public static explicit operator ulong(Rgb48 instance)
        => instance.AsUlong;

    /// <summary>Cast an instance of unsigned long integer to an instance of RGB48</summary>
    /// <param name="instance">The instance to cast.</param>
    /// <returns>The casted instance as RGB48.</returns>
    public static explicit operator Rgb48(ulong instance)
        => new(instance);
    
    /// <summary>Represents the color Pink as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Pink = (Rgb48)0xffffc0cb;
    /// <summary>Represents the color Crimson as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Crimson = (Rgb48)0xffdc143c;
    /// <summary>Represents the color Red as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Red = (Rgb48)0xffff0000;
    /// <summary>Represents the color Maroon as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Maroon = (Rgb48)0xff800000;
    /// <summary>Represents the color Brown as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Brown = (Rgb48)0xffa52a2a;
    /// <summary>Represents the color MistyRose as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 MistyRose = (Rgb48)0xffffe4e1;
    /// <summary>Represents the color Salmon as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Salmon = (Rgb48)0xfffa8072;
    /// <summary>Represents the color Coral as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Coral = (Rgb48)0xffff7f50;
    /// <summary>Represents the color OrangeRed as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 OrangeRed = (Rgb48)0xffff4500;
    /// <summary>Represents the color Chocolate as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Chocolate = (Rgb48)0xffd2691e;
    /// <summary>Represents the color Orange as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Orange = (Rgb48)0xffe1d79d;
    /// <summary>Represents the color Gold as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Gold = (Rgb48)0xffffd700;
    /// <summary>Represents the color Ivory as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Ivory = (Rgb48)0xfffffff0;
    /// <summary>Represents the color Yellow as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Yellow = (Rgb48)0xffffff00;
    /// <summary>Represents the color Olive as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Olive = (Rgb48)0xff808000;
    /// <summary>Represents the color YellowGreen as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 YellowGreen = (Rgb48)0xff9acd32;
    /// <summary>Represents the color LawnGreen as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 LawnGreen = (Rgb48)0xff7cfc00;
    /// <summary>Represents the color Chartreuse as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Chartreuse = (Rgb48)0xff7fff00;
    /// <summary>Represents the color Lime as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Lime = (Rgb48)0xff00ff00;
    /// <summary>Represents the color Green as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Green = (Rgb48)0xff008000;
    /// <summary>Represents the color SpringGreen as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 SpringGreen = (Rgb48)0xff00ff7f;
    /// <summary>Represents the color Aquamarine as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Aquamarine = (Rgb48)0xff7fffd4;
    /// <summary>Represents the color Turquoise as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Turquoise = (Rgb48)0xff40e0d0;
    /// <summary>Represents the color Azure as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Azure = (Rgb48)0xfff0ffff;
    /// <summary>Represents the color AquaCyan as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 AquaCyan = (Rgb48)0xffc2c2c2;
    /// <summary>Represents the color Teal as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Teal = (Rgb48)0xff008080;
    /// <summary>Represents the color Lavender as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Lavender = (Rgb48)0xffe6e6fa;
    /// <summary>Represents the color Blue as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Blue = (Rgb48)0xff0000ff;
    /// <summary>Represents the color Navy as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Navy = (Rgb48)0xff000080;
    /// <summary>Represents the color BlueViolet as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 BlueViolet = (Rgb48)0xff8a2be2;
    /// <summary>Represents the color Indigo as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Indigo = (Rgb48)0xff4b0082;
    /// <summary>Represents the color DarkViolet as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 DarkViolet = (Rgb48)0xff9400d3;
    /// <summary>Represents the color Plum as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Plum = (Rgb48)0xffdda0dd;
    /// <summary>Represents the color Magenta as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Magenta = (Rgb48)0xffff00ff;
    /// <summary>Represents the color Purple as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Purple = (Rgb48)0xffbf9dbf;
    /// <summary>Represents the color RedViolet as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 RedViolet = (Rgb48)0xffc71585;
    /// <summary>Represents the color Tan as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Tan = (Rgb48)0xffd2b48c;
    /// <summary>Represents the color Beige as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Beige = (Rgb48)0xfff5f5dc;
    /// <summary>Represents the color SlateGray as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 SlateGray = (Rgb48)0xff708090;
    /// <summary>Represents the color DarkSlateGray as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 DarkSlateGray = (Rgb48)0xff2f4f4f;
    /// <summary>Represents the color White as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 White = (Rgb48)0xffffffff;
    /// <summary>Represents the color SmokeWhite as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 SmokeWhite = (Rgb48)0xfff5f5f5;
    /// <summary>Represents the color LightGray as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 LightGray = (Rgb48)0xffd3d3d3;
    /// <summary>Represents the color Silver as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Silver = (Rgb48)0xffc0c0c0;
    /// <summary>Represents the color DarkGray as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 DarkGray = (Rgb48)0xffa9a9a9;
    /// <summary>Represents the color Gray as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Gray = (Rgb48)0xff808080;
    /// <summary>Represents the color DimGray as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 DimGray = (Rgb48)0xff696969;
    /// <summary>Represents the color Black as RGB48 instance</summary>
    [PublicAPI] public static readonly Rgb48 Black = (Rgb48)0xff000000;
}