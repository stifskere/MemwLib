using JetBrains.Annotations;

namespace MemwLib.Http.Types.SSL;

/// <summary>This enum lets you decide how is the server going to mainly behave.</summary>
[PublicAPI]
public enum SslBehavior
{
    /// <summary>
    /// Whether a custom certificate was specified, if not
    /// found, the server is going to generate a self signed one.
    /// </summary>
    AlwaysFindAndUseCertificate,
    
    /// <summary>If a certificate is not found use HTTP instead of HTTPS.</summary>
    DoNotUseCertificateIfNotFound
}