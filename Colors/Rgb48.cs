using JetBrains.Annotations;

namespace MemwLib.Colors;

public class Rgb48
{
    [PublicAPI]
    public ushort R { get; set; }
    
    [PublicAPI]
    public ushort G { get; set; }
    
    [PublicAPI]
    public ushort B { get; set; }

    public Rgb48(ushort r, ushort g, ushort b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Rgb48(ulong color) 
        : this((ushort)((color >> 32) & 0xFFFF), (ushort)((color >> 16) & 0xFFFF), (ushort)(color & 0xFFFF))
    {
        if (color > 0xFFFFFFFFFFFF)
            throw new ArgumentException("Color value exceeds maximum unsigned short value.", nameof(color));
    }

    [PublicAPI]
    public virtual ulong ToUlong()
        => (ulong)R << 32 | (ulong)G << 16 | B;

    public static explicit operator ulong(Rgb48 instance)
        => instance.ToUlong();

    public static explicit operator Rgb48(ulong instance)
        => new(instance);
    
    [PublicAPI] public static readonly Rgb48 Pink = (Rgb48)0xffffc0cb;
    [PublicAPI] public static readonly Rgb48 Crimson = (Rgb48)0xffdc143c;
    [PublicAPI] public static readonly Rgb48 Red = (Rgb48)0xffff0000;
    [PublicAPI] public static readonly Rgb48 Maroon = (Rgb48)0xff800000;
    [PublicAPI] public static readonly Rgb48 Brown = (Rgb48)0xffa52a2a;
    [PublicAPI] public static readonly Rgb48 MistyRose = (Rgb48)0xffffe4e1;
    [PublicAPI] public static readonly Rgb48 Salmon = (Rgb48)0xfffa8072;
    [PublicAPI] public static readonly Rgb48 Coral = (Rgb48)0xffff7f50;
    [PublicAPI] public static readonly Rgb48 OrangeRed = (Rgb48)0xffff4500;
    [PublicAPI] public static readonly Rgb48 Chocolate = (Rgb48)0xffd2691e;
    [PublicAPI] public static readonly Rgb48 Orange = (Rgb48)0xffe1d79d;
    [PublicAPI] public static readonly Rgb48 Gold = (Rgb48)0xffffd700;
    [PublicAPI] public static readonly Rgb48 Ivory = (Rgb48)0xfffffff0;
    [PublicAPI] public static readonly Rgb48 Yellow = (Rgb48)0xffffff00;
    [PublicAPI] public static readonly Rgb48 Olive = (Rgb48)0xff808000;
    [PublicAPI] public static readonly Rgb48 YellowGreen = (Rgb48)0xff9acd32;
    [PublicAPI] public static readonly Rgb48 LawnGreen = (Rgb48)0xff7cfc00;
    [PublicAPI] public static readonly Rgb48 Chartreuse = (Rgb48)0xff7fff00;
    [PublicAPI] public static readonly Rgb48 Lime = (Rgb48)0xff00ff00;
    [PublicAPI] public static readonly Rgb48 Green = (Rgb48)0xff008000;
    [PublicAPI] public static readonly Rgb48 SpringGreen = (Rgb48)0xff00ff7f;
    [PublicAPI] public static readonly Rgb48 Aquamarine = (Rgb48)0xff7fffd4;
    [PublicAPI] public static readonly Rgb48 Turquoise = (Rgb48)0xff40e0d0;
    [PublicAPI] public static readonly Rgb48 Azure = (Rgb48)0xfff0ffff;
    [PublicAPI] public static readonly Rgb48 AquaCyan = (Rgb48)0xffc2c2c2;
    [PublicAPI] public static readonly Rgb48 Teal = (Rgb48)0xff008080;
    [PublicAPI] public static readonly Rgb48 Lavender = (Rgb48)0xffe6e6fa;
    [PublicAPI] public static readonly Rgb48 Blue = (Rgb48)0xff0000ff;
    [PublicAPI] public static readonly Rgb48 Navy = (Rgb48)0xff000080;
    [PublicAPI] public static readonly Rgb48 BlueViolet = (Rgb48)0xff8a2be2;
    [PublicAPI] public static readonly Rgb48 Indigo = (Rgb48)0xff4b0082;
    [PublicAPI] public static readonly Rgb48 DarkViolet = (Rgb48)0xff9400d3;
    [PublicAPI] public static readonly Rgb48 Plum = (Rgb48)0xffdda0dd;
    [PublicAPI] public static readonly Rgb48 Magenta = (Rgb48)0xffff00ff;
    [PublicAPI] public static readonly Rgb48 Purple = (Rgb48)0xffbf9dbf;
    [PublicAPI] public static readonly Rgb48 RedViolet = (Rgb48)0xffc71585;
    [PublicAPI] public static readonly Rgb48 Tan = (Rgb48)0xffd2b48c;
    [PublicAPI] public static readonly Rgb48 Beige = (Rgb48)0xfff5f5dc;
    [PublicAPI] public static readonly Rgb48 SlateGray = (Rgb48)0xff708090;
    [PublicAPI] public static readonly Rgb48 DarkSlateGray = (Rgb48)0xff2f4f4f;
    [PublicAPI] public static readonly Rgb48 White = (Rgb48)0xffffffff;
    [PublicAPI] public static readonly Rgb48 SmokeWhite = (Rgb48)0xfff5f5f5;
    [PublicAPI] public static readonly Rgb48 LightGray = (Rgb48)0xffd3d3d3;
    [PublicAPI] public static readonly Rgb48 Silver = (Rgb48)0xffc0c0c0;
    [PublicAPI] public static readonly Rgb48 DarkGray = (Rgb48)0xffa9a9a9;
    [PublicAPI] public static readonly Rgb48 Gray = (Rgb48)0xff808080;
    [PublicAPI] public static readonly Rgb48 DimGray = (Rgb48)0xff696969;
    [PublicAPI] public static readonly Rgb48 Black = (Rgb48)0xff000000;
}