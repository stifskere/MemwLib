using JetBrains.Annotations;

namespace MemwLib.Colors;

/// <summary>32-bit RGBA representation</summary>
public class Rgb32 : Rgb24
{
    /// <summary>The opacity value of this instance.</summary>
    [PublicAPI]
    public byte A { get; set; }
    
    /// <summary>The RGBA values constructor</summary>
    /// <inheritdoc cref="Rgb24(byte,byte,byte)"/>
    /// <param name="a">The opacity value.</param>
#pragma warning disable CS1573
    public Rgb32(byte r, byte g, byte b, byte a) : base(r, g, b)
#pragma warning restore CS1573
    {
        A = a;
    }

    /// <inheritdoc cref="Rgb24(uint)"/>
    public Rgb32(uint color) : base(color >> 8)
    {
        A = (byte)(color & 0xFF);
    }

    /// <summary>Rgb24 cast constructor, constructs an instance of RGB32 from an instance of RGB24.</summary>
    /// <param name="color">The instance to cast from.</param>
    public Rgb32(Rgb24 color) : this(255 | ((uint)color << 8)) { }

    /// <inheritdoc cref="Rgb24.AsUint"/>
    public override uint AsUint
        => (uint)(R << 24 | G << 16 | B << 8 | A);

    /// <summary>Cast an instance of RGB32 to an instance of unsigned integer.</summary>
    /// <inheritdoc cref="Rgb24.op_Explicit(Rgb24)"/>
    public static explicit operator uint(Rgb32 instance)
        => instance.AsUint;

    /// <summary>Cast an instance of unsigned integer to an instance of RGB32</summary>
    /// <inheritdoc cref="Rgb24.op_Explicit(uint)"/>
    /// <returns>The casted instance as RGB32.</returns>
    public static explicit operator Rgb32(uint instance)
        => new(instance);
}