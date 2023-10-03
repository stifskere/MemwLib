using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using JetBrains.Annotations;
using MemwLib.Http.Types.Builders;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http;

public static class HttpRequests
{
    private static X509Certificate _cert;
    
    static HttpRequests()
    {
        _cert = new X509Certificate("./cert.pem", "./key.pem");
    }
    
    [PublicAPI]
    public static async Task<ResponseEntity> CreateRequest(RequestBuilder request)
    {
        (CompleteUri uri, RequestEntity entity) = request.Build();
        
        using TcpClient client = new(uri.Name, uri.Port);

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
                TargetHost = uri.Name,
                ClientCertificates = new X509CertificateCollection(new []{_cert}),
                AllowRenegotiation = true
            });
            await httpsStream.WriteAsync(entity.ToArray());
            await httpsStream.CopyToAsync(tempStream);
        }
        
        client.Close();
        
        return new ResponseEntity(Encoding.ASCII.GetString(tempStream.ToArray()));
    }
}