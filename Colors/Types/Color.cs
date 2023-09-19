using JetBrains.Annotations;

namespace MemwLib.Colors.Types;

public class Color
{
    [PublicAPI]
    public byte R { get; set; }
    [PublicAPI]
    public byte G { get; set; }
    [PublicAPI]
    public byte B { get; set; }

    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Color(uint color)
    {
        if (color > 0xFFFFFF)
            throw new ArgumentException("Color value exceeds maximum byte value.", nameof(color));
        
        R = Convert.ToByte((color >> 16) & 0xFF);
        G = Convert.ToByte((color >> 8) & 0xFF);
        B = Convert.ToByte(color & 0xFF);
    }

    [PublicAPI]
    public uint ToUInt() 
        => (uint)(R << 16 | G << 8 | B);

    public static implicit operator uint(Color instance)
        => instance.ToUInt();

    public static implicit operator Color(uint instance)
        => new(instance);

    [PublicAPI] public static readonly Color Pink = 0xffc0cb;
    [PublicAPI] public static readonly Color Crimson = 0xdc143c;
    [PublicAPI] public static readonly Color Red = 0xff0000;
    [PublicAPI] public static readonly Color Maroon = 0x800000;
    [PublicAPI] public static readonly Color Brown = 0xa52a2a;
    [PublicAPI] public static readonly Color MistyRose = 0xffe4e1;
    [PublicAPI] public static readonly Color Salmon = 0xfa8072;
    [PublicAPI] public static readonly Color Coral = 0xff7f50;
    [PublicAPI] public static readonly Color OrangeRed = 0xff4500;
    [PublicAPI] public static readonly Color Chocolate = 0xd2691e;
    [PublicAPI] public static readonly Color Orange = 0xe1d79d;
    [PublicAPI] public static readonly Color Gold = 0xffd700;
    [PublicAPI] public static readonly Color Ivory = 0xfffff0;
    [PublicAPI] public static readonly Color Yellow = 0xffff00;
    [PublicAPI] public static readonly Color Olive = 0x808000;
    [PublicAPI] public static readonly Color YellowGreen = 0x9acd32;
    [PublicAPI] public static readonly Color LawnGreen = 0x7cfc00;
    [PublicAPI] public static readonly Color Charteuse = 0x7fff00;
    [PublicAPI] public static readonly Color Lime = 0x00ff00;
    [PublicAPI] public static readonly Color Green = 0x008000;
    [PublicAPI] public static readonly Color SpringGreen = 0x00ff7f;
    [PublicAPI] public static readonly Color Aquamarine = 0x7fffd4;
    [PublicAPI] public static readonly Color Toruoise = 0x40e0d0;
    [PublicAPI] public static readonly Color Azure = 0xf0ffff;
    [PublicAPI] public static readonly Color AquaCyan = 0xc2c2c2;
    [PublicAPI] public static readonly Color Teal = 0x008080;
    [PublicAPI] public static readonly Color Lavender = 0xe6e6fa;
    [PublicAPI] public static readonly Color Blue = 0x0000ff;
    [PublicAPI] public static readonly Color Navy = 0x000080;
    [PublicAPI] public static readonly Color BlueViolet = 0x8a2be2;
    [PublicAPI] public static readonly Color Indigo = 0x4b0082;
    [PublicAPI] public static readonly Color DarkViolet = 0x9400d3;
    [PublicAPI] public static readonly Color Plum = 0xdda0dd;
    [PublicAPI] public static readonly Color Magenta = 0xff00ff;
    [PublicAPI] public static readonly Color Purple = 0xbf9dbf;
    [PublicAPI] public static readonly Color RedViolet = 0xc71585;
    [PublicAPI] public static readonly Color Tan = 0xd2b48c;
    [PublicAPI] public static readonly Color Beige = 0xf5f5dc;
    [PublicAPI] public static readonly Color SlateGray = 0x708090;
    [PublicAPI] public static readonly Color DarkSlateGray = 0x2f4f4f;
    [PublicAPI] public static readonly Color White = 0xffffff;
    [PublicAPI] public static readonly Color SmokeWhite = 0xf5f5f5;
    [PublicAPI] public static readonly Color LightGray = 0xd3d3d3;
    [PublicAPI] public static readonly Color Silver = 0xc0c0c0;
    [PublicAPI] public static readonly Color DarkGray = 0xa9a9a9;
    [PublicAPI] public static readonly Color Gray = 0x808080;
    [PublicAPI] public static readonly Color DimGray = 0x696969;
    [PublicAPI] public static readonly Color Black = 0x000000;
}