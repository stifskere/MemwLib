using JetBrains.Annotations;

namespace MemwLib.Colors;

/// <summary>64-bit RGBA representation.</summary>
public class Rgb64 : Rgb48
{
    /// <summary>The opacity value.</summary>
    [PublicAPI]
    public ushort A { get; set; }

    /// <inheritdoc cref="Rgb32(byte, byte, byte, byte)"/>
    public Rgb64(ushort r, ushort g, ushort b, ushort a) : base(r, g, b)
    {
        A = a;
    }

    /// <summary>The unsigned integer value constructor.</summary>
    /// <param name="color">The decimal representation of the color.</param>
    public Rgb64(ulong color) : base(color >> 16)
    {
        A = (ushort)(color & 0xFFFF);
    }

    /// <summary>Rgb48 cast constructor, constructs an instance of RGB64 from an instance of RGB48.</summary>
    /// <param name="color">The instance to cast from.</param>
    public Rgb64(Rgb48 color) : this(65535 | ((ulong)color << 16)) { }

    /// <inheritdoc cref="Rgb48.AsUlong"/>
    public override ulong AsUlong
        => (ulong)R << 48 | (ulong)G << 32 | (ulong)B << 16 | A;

    /// <summary>Cast an instance of RGB64 to an instance of unsigned long integer.</summary>
    /// <inheritdoc cref="Rgb48.op_Explicit(Rgb48)"/>
    /// <returns>The casted instance as unsigned long integer.</returns>
    public static explicit operator ulong(Rgb64 instance)
        => instance.AsUlong;

    /// <summary>Cast an instance of unsigned long integer to an instance of RGB64</summary>
    /// <inheritdoc cref="Rgb48.op_Explicit(ulong)"/>
    /// <returns>The casted instance as RGB64.</returns>
    public static explicit operator Rgb64(ulong instance)
        => new(instance);
}