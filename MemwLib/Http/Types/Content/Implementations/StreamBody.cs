using System.Text;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content.Implementations;

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
    public static IBody ParseImpl(string content) 
        => new StreamBody(new MemoryStream(Encoding.UTF8.GetBytes(content)));

    /// <inheritdoc cref="IBody.ToRaw"/>
    public string ToRaw()
    {
        _stream.Seek(0, SeekOrigin.Begin);
        using StreamReader reader = new(_stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_closeOnFinish)
            _stream.Dispose();
    }
}