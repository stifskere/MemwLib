using System.Net.Sockets;

namespace MemwLib.Http.Tls;

public class SslStream : NetworkStream
{
    public SslStream(Socket socket) : base(socket) { }

    public SslStream(Socket socket, bool ownsSocket) : base(socket, ownsSocket) { }

    public SslStream(Socket socket, FileAccess access) : base(socket, access) { }

    public SslStream(Socket socket, FileAccess access, bool ownsSocket) : base(socket, access, ownsSocket) { }
}