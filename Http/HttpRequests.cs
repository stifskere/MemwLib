using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using JetBrains.Annotations;
using MemwLib.Http.Types.Builders;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http;

/// <summary>Class that statically holds HTTP request methods.</summary>
public static class HttpRequests
{
    
    /// <summary>Sends an HTTP request based on the request builder parameter.</summary>
    /// <param name="request">The request data.</param>
    /// <returns>A response from the server</returns>
    /// <exception cref="SocketException">An error occurred while trying to access the socket.</exception>
    [PublicAPI]
    public static async Task<ResponseEntity> CreateRequest(RequestBuilder request)
    {
        (CompleteUri uri, RequestEntity entity) = request.Build();

        entity.Headers["Host"] = $"{uri.HostName}:{uri.Port}";
        using TcpClient client = new(uri.HostName, uri.Port);

        using MemoryStream tempStream = new();
        
        if (uri.Protocol == Protocol.Http)
        {
            await using NetworkStream httpStream = client.GetStream();
            await httpStream.WriteAsync(entity.ToArray());
            await httpStream.CopyToAsync(tempStream);
        }
        else
        {
            await using SslStream httpsStream = new SslStream(client.GetStream(), false);
            await httpsStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = uri.HostName
            });
            
            await httpsStream.WriteAsync(entity.ToArray());
            await httpsStream.CopyToAsync(tempStream);
        }
        
        client.Close();
        
        return new ResponseEntity(Encoding.ASCII.GetString(tempStream.ToArray()));
    }
}