using JetBrains.Annotations;

namespace MemwLib.Colors;

public class Rgb32 : Rgb24
{
    [PublicAPI]
    public byte A { get; set; }
    
    public Rgb32(byte r, byte g, byte b, byte a) : base(r, g, b)
    {
        A = a;
    }

    public Rgb32(uint color) : base(color >> 8)
    {
        A = (byte)(color & 0xFF);
    }

    public Rgb32(Rgb24 color) : this((uint)((255 << 24) | (uint)color)) { }

    public override uint ToUInt()
        => (uint)(R << 24 | G << 16 | B << 8 | A);

    public static explicit operator uint(Rgb32 instance)
        => instance.ToUInt();

    public static explicit operator Rgb32(uint instance)
        => new(instance);
}