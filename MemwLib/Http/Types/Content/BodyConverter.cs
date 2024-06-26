using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using MemwLib.CoreUtils;

namespace MemwLib.Http.Types.Content;

/// <summary>
/// This class is intended to add interoperability
/// between strings and IBody implementations
/// </summary>
[PublicAPI]
public class BodyConverter : IDisposable
{
    /// <summary>The content type this BodyConverter is holding.</summary>
    public string ContentType { get; }
    
    /// <summary>The body contained by this converter.</summary>
    public MemoryStream Stream { get; }

    /// <summary>Whether the current body is empty or not.</summary>
    public bool IsEmpty => Length == 0;

    /// <summary>Gets the length of the body in a raw format.</summary>
    public long Length => Stream.Length;
    
    internal static BodyConverter Empty => new(Array.Empty<byte>());
    
    internal BodyConverter(byte[] raw)
    {
        Stream = new MemoryStream(raw);
        ContentType = IsEmpty ? "none/none" : "text/plain";
    }

    internal BodyConverter(IBody? body) : this(body?.ToArray() ?? Array.Empty<byte>())
    {
        ContentType = body?.ContentType ?? "none/none";
    }
    
    /// <summary>Reads the current BodyConverter instance as a body instance.</summary>
    /// <typeparam name="TBody">The type of body to convert to.</typeparam>
    /// <returns>
    /// An instance of TBody based on this converter's
    /// raw string or null if there was no body in the first place.
    /// </returns>
    public TBody ReadAs<TBody>() where TBody : IBody 
        => IBody.Parse<TBody>(Stream);

    /// <summary>Tries to read the current body converter instance as a body instance.</summary>
    /// <param name="body">The result of this conversion.</param>
    /// <typeparam name="TBody">The type of body to convert to.</typeparam>
    /// <returns>A boolean instance whether the body could be converted or not.</returns>
    public bool TryReadAs<TBody>([NotNullWhen(true)] out TBody? body) where TBody : IBody 
        => IBody.TryParse(Stream, out body);

    /// <inheritdoc cref="Object.ToString"/>
    public override string ToString()
        => Stream.GetRaw();

    /// <summary>Get the body contained in this converter as an array.</summary>
    /// <returns>The converted inner body.</returns>
    public byte[] ToArray()
    {
        byte[] result = new byte[Length];
        Stream.Seek(0, SeekOrigin.Begin);
        _ = Stream.Read(result, 0, (int)Length);
        return result;
    }
    
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Stream.Dispose();
    }
}