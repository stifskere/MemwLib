using System.Net;
using System.Net.Sockets;
using System.Text;
using JetBrains.Annotations;
using MemwLib.Http.Types.Builders;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Routes;
using Uri = MemwLib.Http.Types.Routes.Uri;

namespace MemwLib.Http;

public static class HttpRequests
{
    [PublicAPI]
    public static async Task<ResponseEntity> CreateRequest(RequestBuilder request)
    {
        (Uri uri, RequestEntity entity) = request.Build();

        if (uri.Protocol == Protocol.Https)
            throw new NotImplementedException("Https is not yet implemented.");
        
        using TcpClient client = new();
        await client.ConnectAsync((IPAddress)uri, (int)uri.Protocol);
        await using NetworkStream communicationStream = client.GetStream();

        await communicationStream.WriteAsync(entity.ToArray());

        using MemoryStream tempStream = new();
        await communicationStream.CopyToAsync(tempStream);
        byte[] responseArray = tempStream.ToArray();
        
        communicationStream.Close();
        client.Close();
        
        return new ResponseEntity(Encoding.ASCII.GetString(responseArray));
    }
}