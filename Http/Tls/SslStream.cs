using System.Net.Sockets;

namespace MemwLib.Http.Tls;

#pragma warning disable CS1591

public class SslStream : NetworkStream
{

    public SslStream(Socket socket) : base(socket)
    {
        throw new NotImplementedException();
    }

    public SslStream(Socket socket, bool ownsSocket) : base(socket, ownsSocket)
    {
        throw new NotImplementedException();
    }

    public SslStream(Socket socket, FileAccess access) : base(socket, access)
    {
        throw new NotImplementedException();
    }

    public SslStream(Socket socket, FileAccess access, bool ownsSocket) : base(socket, access, ownsSocket)
    {
        throw new NotImplementedException();
    }
}

#pragma warning restore CS1591