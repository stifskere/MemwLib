using JetBrains.Annotations;

namespace MemwLib.Colors;

public class Rgb64 : Rgb48
{
    [PublicAPI]
    public ushort A { get; set; }

    public Rgb64(ushort r, ushort g, ushort b, ushort a) : base(r, g, b)
    {
        A = a;
    }

    public Rgb64(ulong color) : base(color >> 16)
    {
        A = (ushort)(color & 0xFFFF);
    }

    public Rgb64(Rgb48 color) : this(((ulong)65535 << 48) | (ulong)color) { }

    public override ulong ToUlong()
        => (ulong)R << 48 | (ulong)G << 32 | (ulong)B << 16 | A;

    public static explicit operator ulong(Rgb64 instance)
        => instance.ToUlong();

    public static explicit operator Rgb64(ulong instance)
        => new(instance);
}