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
            Headers = config.Headers,
            Path =
            {
                Fragment = null
            }
        };

        if (target is { User: not null, Password: not null })
            request.Headers.Add(
                "Authorization",
                $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{target.User}:{target.Password}"))}"
            );

        if (!request.Headers.Contains("Content-Length"))
            request.Headers.Add("Content-Length", request.Body.Length.ToString());
        
        request.Headers.Add("Host", target.HostName);
        using TcpClient client = new(target.HostName, target.Port);
        
        if (target.Protocol == Protocol.Http)
        {
            await using NetworkStream httpStream = client.GetStream();
            await httpStream.WriteAsync(request.ToArray());
            await httpStream.FlushAsync();
            
            return new ResponseEntity(new StreamReader(httpStream));
        }

        await using SslStream httpsStream = new SslStream(client.GetStream(), false);
        await httpsStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
        {
            TargetHost = target.HostName,
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
        });
            
        await httpsStream.WriteAsync(request.ToArray());
        await httpsStream.FlushAsync();
        
        return new ResponseEntity(new StreamReader(httpsStream));
    }
}