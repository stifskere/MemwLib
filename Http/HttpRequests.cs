using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using JetBrains.Annotations;
using MemwLib.Http.Types.Configuration;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http;

/// <summary>Class that statically holds HTTP request methods.</summary>
public static class HttpRequests
{
    /// <summary>Sends an HTTP request based on the request builder parameter.</summary>
    /// <param name="config">Request configuration parameters.</param>
    /// <returns>A response from the server.</returns>
    /// <exception cref="SocketException">An error occurred while trying to access the socket.</exception>
    [PublicAPI]
    public static async Task<ResponseEntity> CreateRequest(HttpRequestConfig config)
    {
        CompleteUri target = new CompleteUri(config.Url);
        RequestEntity request = new(config.Method, target, config.Body)
        {
            Headers = config.Headers
        };

        request.Headers["Host"] = target.HostName;
        using TcpClient client = new(target.HostName, target.Port);

        ResponseEntity result;
        
        if (target.Protocol == Protocol.Http)
        {
            await using NetworkStream httpStream = client.GetStream();
            await httpStream.WriteAsync(request.ToArray());
            await httpStream.FlushAsync();

            result = new ResponseEntity(new StreamReader(httpStream));
        }
        else
        {
            await using SslStream httpsStream = new SslStream(client.GetStream(), true);
            await httpsStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = target.HostName,
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            });
            
            await httpsStream.WriteAsync(request.ToArray());
            await httpsStream.FlushAsync();

            result = new ResponseEntity(new StreamReader(httpsStream));
        }
        
        client.Close();
        
        return result;
    }
}