using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content.Common;

/// <summary>Read a body as a stream</summary>
[PublicAPI]
public class StreamBody : IBody, IDisposable
{
    /// <inheritdoc cref="IBody.ContentType"/>
    public string ContentType { get; }

    private Stream _stream;
    private bool _closeOnFinish;

    /// <summary>Create a new instance of Stream body</summary>
    /// <param name="stream">The stream to handle.</param>
    /// <param name="contentType">The stream mime type.</param>
    /// <param name="closeOnFinish">Whether to close or not on dispose this instance.</param>
    public StreamBody(Stream stream, string contentType, bool closeOnFinish = false)
    {
        _stream = stream;
        _closeOnFinish = closeOnFinish;
        ContentType = contentType;
    }

    /// <summary>Create a new instance of Stream body</summary>
    /// <param name="stream">The stream to handle.</param>
    /// <param name="closeOnFinish">Whether to close or not on dispose this instance.</param>
    public StreamBody(Stream stream, bool closeOnFinish = false) : this(stream, "application/octet-stream", closeOnFinish) { }

    /// <inheritdoc cref="IBody.ParseImpl"/>
    public static IBody ParseImpl(MemoryStream content) 
        => new StreamBody(content, true);

    /// <inheritdoc cref="IBody.ToArray"/>
    public byte[] ToArray()
    {
        _stream.Seek(0, SeekOrigin.Begin);
        byte[] result = new byte[_stream.Length];
        _ = _stream.Read(result, 0, result.Length);
        return result;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_closeOnFinish)
            _stream.Dispose();
    }
}