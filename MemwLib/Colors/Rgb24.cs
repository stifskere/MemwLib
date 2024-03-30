using JetBrains.Annotations;

namespace MemwLib.Colors;

/// <summary>24-bit RGB representation.</summary>
public class Rgb24
{
    /// <summary>The red value of this instance.</summary>
    [PublicAPI]
    public byte R { get; set; }
    
    /// <summary>The green value of this instance.</summary>
    [PublicAPI]
    public byte G { get; set; }
    
    /// <summary>The blue value of this instance.</summary>
    [PublicAPI]
    public byte B { get; set; }

    /// <summary>The RGB values constructor.</summary>
    /// <param name="r">the red value.</param>
    /// <param name="g">the green value.</param>
    /// <param name="b">the blue value.</param>
    public Rgb24(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    /// <summary>The unsigned integer value constructor.</summary>
    /// <param name="color">The decimal representation of the color.</param>
    /// <exception cref="ArgumentException">The color exceeds the 0xFFFFFF value.</exception>
    public Rgb24(uint color) : this((byte)((color >> 16) & 0xFF), (byte)((color >> 8) & 0xFF), (byte)(color & 0xFF))
    {
        if (color > 0xFFFFFF)
            throw new ArgumentException("Color value exceeds maximum byte value.", nameof(color));
    }

    /// <summary>Rgb32 cast constructor, constructs an instance of RGB24 from an instance of RGB32.</summary>
    /// <param name="color">The instance to cast from.</param>
    public Rgb24(Rgb32 color) : this((uint)color >> 8) { }

    /// <summary>Obtain the color decimal value as an unsigned integer instance.</summary>
    [PublicAPI]
    public virtual uint AsUint 
        => (uint)(R << 16 | G << 8 | B);

    /// <summary>Cast an instance of RGB24 to an instance of unsigned integer.</summary>
    /// <param name="instance">The instance to cast.</param>
    /// <returns>The casted instance as unsigned integer.</returns>
    public static explicit operator uint(Rgb24 instance)
        => instance.AsUint;

    /// <summary>Cast an instance of unsigned integer to an instance of RGB24</summary>
    /// <param name="instance">The instance to cast.</param>
    /// <returns>The casted instance as RGB24.</returns>
    public static explicit operator Rgb24(uint instance)
        => new(instance);

    
    /// <summary>Represents the color Pink as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Pink = (Rgb24)0xffc0cb;
    /// <summary>Represents the color Crimson as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Crimson = (Rgb24)0xdc143c;
    /// <summary>Represents the color Red as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Red = (Rgb24)0xff0000;
    /// <summary>Represents the color Maroon as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Maroon = (Rgb24)0x800000;
    /// <summary>Represents the color Brown as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Brown = (Rgb24)0xa52a2a;
    /// <summary>Represents the color MistyRose as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 MistyRose = (Rgb24)0xffe4e1;
    /// <summary>Represents the color Salmon as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Salmon = (Rgb24)0xfa8072;
    /// <summary>Represents the color Coral as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Coral = (Rgb24)0xff7f50;
    /// <summary>Represents the color OrangeRed as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 OrangeRed = (Rgb24)0xff4500;
    /// <summary>Represents the color Chocolate as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Chocolate = (Rgb24)0xd2691e;
    /// <summary>Represents the color Orange as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Orange = (Rgb24)0xe1d79d;
    /// <summary>Represents the color Gold as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Gold = (Rgb24)0xffd700;
    /// <summary>Represents the color Ivory as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Ivory = (Rgb24)0xfffff0;
    /// <summary>Represents the color Yellow as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Yellow = (Rgb24)0xffff00;
    /// <summary>Represents the color Olive as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Olive = (Rgb24)0x808000;
    /// <summary>Represents the color YellowGreen as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 YellowGreen = (Rgb24)0x9acd32;
    /// <summary>Represents the color LawnGreen as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 LawnGreen = (Rgb24)0x7cfc00;
    /// <summary>Represents the color Chartreuse as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Chartreuse = (Rgb24)0x7fff00;
    /// <summary>Represents the color Lime as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Lime = (Rgb24)0x00ff00;
    /// <summary>Represents the color Green as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Green = (Rgb24)0x008000;
    /// <summary>Represents the color SpringGreen as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 SpringGreen = (Rgb24)0x00ff7f;
    /// <summary>Represents the color Aquamarine as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Aquamarine = (Rgb24)0x7fffd4;
    /// <summary>Represents the color Turquoise as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Turquoise = (Rgb24)0x40e0d0;
    /// <summary>Represents the color Azure as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Azure = (Rgb24)0xf0ffff;
    /// <summary>Represents the color AquaCyan as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 AquaCyan = (Rgb24)0xc2c2c2;
    /// <summary>Represents the color Teal as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Teal = (Rgb24)0x008080;
    /// <summary>Represents the color Lavender as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Lavender = (Rgb24)0xe6e6fa;
    /// <summary>Represents the color Blue as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Blue = (Rgb24)0x0000ff;
    /// <summary>Represents the color Navy as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Navy = (Rgb24)0x000080;
    /// <summary>Represents the color BlueViolet as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 BlueViolet = (Rgb24)0x8a2be2;
    /// <summary>Represents the color Indigo as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Indigo = (Rgb24)0x4b0082;
    /// <summary>Represents the color DarkViolet as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 DarkViolet = (Rgb24)0x9400d3;
    /// <summary>Represents the color Plum as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Plum = (Rgb24)0xdda0dd;
    /// <summary>Represents the color Magenta as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Magenta = (Rgb24)0xff00ff;
    /// <summary>Represents the color Purple as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Purple = (Rgb24)0xbf9dbf;
    /// <summary>Represents the color RedViolet as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 RedViolet = (Rgb24)0xc71585;
    /// <summary>Represents the color Tan as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Tan = (Rgb24)0xd2b48c;
    /// <summary>Represents the color Beige as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Beige = (Rgb24)0xf5f5dc;
    /// <summary>Represents the color SlateGray as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 SlateGray = (Rgb24)0x708090;
    /// <summary>Represents the color DarkSlateGray as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 DarkSlateGray = (Rgb24)0x2f4f4f;
    /// <summary>Represents the color White as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 White = (Rgb24)0xffffff;
    /// <summary>Represents the color SmokeWhite as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 SmokeWhite = (Rgb24)0xf5f5f5;
    /// <summary>Represents the color LightGray as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 LightGray = (Rgb24)0xd3d3d3;
    /// <summary>Represents the color Silver as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Silver = (Rgb24)0xc0c0c0;
    /// <summary>Represents the color DarkGray as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 DarkGray = (Rgb24)0xa9a9a9;
    /// <summary>Represents the color Gray as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Gray = (Rgb24)0x808080;
    /// <summary>Represents the color DimGray as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 DimGray = (Rgb24)0x696969;
    /// <summary>Represents the color Black as RGB24.</summary>
    [PublicAPI] public static readonly Rgb24 Black = (Rgb24)0x000000;
}