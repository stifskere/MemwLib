using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using JetBrains.Annotations;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http;

/// <summary>Class that statically holds HTTP request methods.</summary>
public static class HttpRequests
{
    /// <summary>Sends an HTTP request based on the request builder parameter.</summary>
    /// <param name="target">Where to send that data.</param>
    /// <param name="request">The request data.</param>
    /// <returns>A response from the server</returns>
    /// <exception cref="SocketException">An error occurred while trying to access the socket.</exception>
    [PublicAPI]
    public static async Task<ResponseEntity> CreateRequest(CompleteUri target, RequestEntity request)
    {
        request.Headers["Host"] = $"{target.HostName}:{target.Port}";
        using TcpClient client = new(target.HostName, target.Port);

        using MemoryStream tempStream = new();
        
        if (target.Protocol == Protocol.Http)
        {
            await using NetworkStream httpStream = client.GetStream();
            await httpStream.WriteAsync(request.ToArray());
            await httpStream.CopyToAsync(tempStream);
        }
        else
        {
            await using SslStream httpsStream = new SslStream(client.GetStream(), false);
            await httpsStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = target.HostName
            });
            
            await httpsStream.WriteAsync(request.ToArray());
            await httpsStream.CopyToAsync(tempStream);
        }
        
        client.Close();
        
        return new ResponseEntity(Encoding.ASCII.GetString(tempStream.ToArray()));
    }
}