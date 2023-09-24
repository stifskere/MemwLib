using JetBrains.Annotations;

namespace MemwLib.Colors.Types;

public class Rgb24
{
    [PublicAPI]
    public byte R { get; set; }
    [PublicAPI]
    public byte G { get; set; }
    [PublicAPI]
    public byte B { get; set; }

    public Rgb24(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Rgb24(uint color) : this((byte)((color >> 16) & 0xFF), (byte)((color >> 8) & 0xFF), (byte)(color & 0xFF))
    {
        if (color > 0xFFFFFF)
            throw new ArgumentException("Color value exceeds maximum byte value.", nameof(color));
    }

    public Rgb24(Rgb32 color) : this((uint)color << 8) { }

    [PublicAPI]
    public virtual uint ToUInt() 
        => (uint)(R << 16 | G << 8 | B);

    public static explicit operator uint(Rgb24 instance)
        => instance.ToUInt();

    public static explicit operator Rgb24(uint instance)
        => new(instance);

    [PublicAPI] public static readonly Rgb24 Pink = (Rgb24)0xffc0cb;
    [PublicAPI] public static readonly Rgb24 Crimson = (Rgb24)0xdc143c;
    [PublicAPI] public static readonly Rgb24 Red = (Rgb24)0xff0000;
    [PublicAPI] public static readonly Rgb24 Maroon = (Rgb24)0x800000;
    [PublicAPI] public static readonly Rgb24 Brown = (Rgb24)0xa52a2a;
    [PublicAPI] public static readonly Rgb24 MistyRose = (Rgb24)0xffe4e1;
    [PublicAPI] public static readonly Rgb24 Salmon = (Rgb24)0xfa8072;
    [PublicAPI] public static readonly Rgb24 Coral = (Rgb24)0xff7f50;
    [PublicAPI] public static readonly Rgb24 OrangeRed = (Rgb24)0xff4500;
    [PublicAPI] public static readonly Rgb24 Chocolate = (Rgb24)0xd2691e;
    [PublicAPI] public static readonly Rgb24 Orange = (Rgb24)0xe1d79d;
    [PublicAPI] public static readonly Rgb24 Gold = (Rgb24)0xffd700;
    [PublicAPI] public static readonly Rgb24 Ivory = (Rgb24)0xfffff0;
    [PublicAPI] public static readonly Rgb24 Yellow = (Rgb24)0xffff00;
    [PublicAPI] public static readonly Rgb24 Olive = (Rgb24)0x808000;
    [PublicAPI] public static readonly Rgb24 YellowGreen = (Rgb24)0x9acd32;
    [PublicAPI] public static readonly Rgb24 LawnGreen = (Rgb24)0x7cfc00;
    [PublicAPI] public static readonly Rgb24 Chartreuse = (Rgb24)0x7fff00;
    [PublicAPI] public static readonly Rgb24 Lime = (Rgb24)0x00ff00;
    [PublicAPI] public static readonly Rgb24 Green = (Rgb24)0x008000;
    [PublicAPI] public static readonly Rgb24 SpringGreen = (Rgb24)0x00ff7f;
    [PublicAPI] public static readonly Rgb24 Aquamarine = (Rgb24)0x7fffd4;
    [PublicAPI] public static readonly Rgb24 Turquoise = (Rgb24)0x40e0d0;
    [PublicAPI] public static readonly Rgb24 Azure = (Rgb24)0xf0ffff;
    [PublicAPI] public static readonly Rgb24 AquaCyan = (Rgb24)0xc2c2c2;
    [PublicAPI] public static readonly Rgb24 Teal = (Rgb24)0x008080;
    [PublicAPI] public static readonly Rgb24 Lavender = (Rgb24)0xe6e6fa;
    [PublicAPI] public static readonly Rgb24 Blue = (Rgb24)0x0000ff;
    [PublicAPI] public static readonly Rgb24 Navy = (Rgb24)0x000080;
    [PublicAPI] public static readonly Rgb24 BlueViolet = (Rgb24)0x8a2be2;
    [PublicAPI] public static readonly Rgb24 Indigo = (Rgb24)0x4b0082;
    [PublicAPI] public static readonly Rgb24 DarkViolet = (Rgb24)0x9400d3;
    [PublicAPI] public static readonly Rgb24 Plum = (Rgb24)0xdda0dd;
    [PublicAPI] public static readonly Rgb24 Magenta = (Rgb24)0xff00ff;
    [PublicAPI] public static readonly Rgb24 Purple = (Rgb24)0xbf9dbf;
    [PublicAPI] public static readonly Rgb24 RedViolet = (Rgb24)0xc71585;
    [PublicAPI] public static readonly Rgb24 Tan = (Rgb24)0xd2b48c;
    [PublicAPI] public static readonly Rgb24 Beige = (Rgb24)0xf5f5dc;
    [PublicAPI] public static readonly Rgb24 SlateGray = (Rgb24)0x708090;
    [PublicAPI] public static readonly Rgb24 DarkSlateGray = (Rgb24)0x2f4f4f;
    [PublicAPI] public static readonly Rgb24 White = (Rgb24)0xffffff;
    [PublicAPI] public static readonly Rgb24 SmokeWhite = (Rgb24)0xf5f5f5;
    [PublicAPI] public static readonly Rgb24 LightGray = (Rgb24)0xd3d3d3;
    [PublicAPI] public static readonly Rgb24 Silver = (Rgb24)0xc0c0c0;
    [PublicAPI] public static readonly Rgb24 DarkGray = (Rgb24)0xa9a9a9;
    [PublicAPI] public static readonly Rgb24 Gray = (Rgb24)0x808080;
    [PublicAPI] public static readonly Rgb24 DimGray = (Rgb24)0x696969;
    [PublicAPI] public static readonly Rgb24 Black = (Rgb24)0x000000;
}