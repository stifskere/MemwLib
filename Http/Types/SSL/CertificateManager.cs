using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MemwLib.Http.Types.SSL;

// I'm not fucking touching this shit again.
internal static class CertificateManager
{
    private const string CertPath = "./MemwLib.pfx";
    private const string CertPassword = "J2&D]u[/Jqr!U=l>fg,)K$}/AZ!U/5YH*z{[4QvZuU;(;Le?X$";
    
    public static X509Certificate2 GenerateOrRetrieveCertificate()
    {
        if (File.Exists(CertPath))
        {
            X509Certificate2 existingCertificate = new X509Certificate2(CertPath, CertPassword);

            if (DateTime.Now < existingCertificate.NotAfter)
                return existingCertificate;
            
            File.Delete(CertPath);
        }
        
        using RSA rsa = RSA.Create(2048);
        
        CertificateRequest request = new(new X500DistinguishedName("CN=MemwLib"), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));

        X509Certificate2 newCertificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
        
        File.WriteAllBytes(CertPath, newCertificate.Export(X509ContentType.Pkcs12, CertPassword));
        
        return newCertificate;
    }
}