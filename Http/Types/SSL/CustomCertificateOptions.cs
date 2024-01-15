using JetBrains.Annotations;

namespace MemwLib.Http.Types.SSL;

/// <summary>This class is meant to specify options for a custom certificate.</summary>
[PublicAPI]
public class CustomCertificateOptions
{
    /// <summary>This is the certificate path, or where to get the pfx file from.</summary>
    public required string CertificatePath { get; init; }
    
    /// <summary>This is the certificate password, it is used to let the server decrypt the certificate.</summary>
    public required string CertificatePassword { get; init; }
}