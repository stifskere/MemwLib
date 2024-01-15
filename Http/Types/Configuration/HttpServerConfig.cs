using System.Net;
using JetBrains.Annotations;
using MemwLib.Http.Types.SSL;

namespace MemwLib.Http.Types.Configuration;

/// <summary>This class serves as configuration for the HttpServer constructor.</summary>
[PublicAPI]
public class HttpServerConfig
{
    /// <summary>The address this server is going to listen to.</summary>
    public IPAddress Address { get; init; } = IPAddress.Any;

    /// <summary>The port this server is going to listen to.</summary>
    public ushort Port { get; init; }

    /// <summary>The current server state, it tells the server if it should show or not debug data.</summary>
    public ServerStates ServerState { get; init; } = ServerStates.Production;
    
    /// <summary>The certificate path this server is going to use.</summary>
    /// <remarks>If null, it's going to use a self signed certificate.</remarks>
    public CustomCertificateOptions? SslCertificate { get; init; }
    
    /// <summary>Defines the server behavior on how it should interact with SSL</summary>
    public SslBehavior SslBehavior { get; init; } = SslBehavior.DoNotUseCertificateIfNotFound;

    /// <inheritdoc cref="HttpServerConfig"/>
    public HttpServerConfig()
    {
        Port = (ushort)(SslCertificate == null && SslBehavior == SslBehavior.DoNotUseCertificateIfNotFound ? 80u : 448u);
    }
}