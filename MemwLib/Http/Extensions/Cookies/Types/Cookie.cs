using System.Data;
using JetBrains.Annotations;
using MemwLib.Http.Types.Routes;

namespace MemwLib.Http.Extensions.Cookies.Types;

/// <summary>
/// Represents a browser cookie, used to parameterize
/// the cookie manager in a more manageable way.
/// </summary>
[PublicAPI]
public struct Cookie()
{
    /// <summary>The cookie identifier.</summary>
    /// <remarks>
    /// Please DO UrlEncode the name if it contains characters
    /// outside the standard range.
    /// </remarks>
    public required string Name { get; init; }
    
    /// <summary>The cookie value or what does it contain.</summary>
    /// <remarks>
    /// Please DO urlEncode the value if it contains characters
    /// outside the standard range.
    /// </remarks>
    public required string Value { get; set; }

    /// <summary>
    /// When does the cookie expire, please, don't set it to a past value.
    /// Use the WithoutCookie method instead.
    /// </summary>
    public DateTime Expires { get; set; } = DateTime.Now + TimeSpan.FromMinutes(20);
    
    /// <summary>
    /// The Domain attribute specifies which
    /// server can receive a cookie.
    /// </summary>
    public string? Domain { get; set; }
    
    /// <summary>
    /// The Path attribute indicates a URL path that must exist in
    /// the requested URL in order to send the Cookie header.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// If present, the cookie will only be
    /// sent over secure (HTTPS) connections.
    /// </summary>
    public bool Secure { get; set; } = false;

    /// <summary>
    /// If present, the cookie will be inaccessible to JavaScript,
    /// helping to mitigate certain types of cross-site scripting (XSS) attacks.
    /// </summary>
    public bool HttpOnly { get; set; } = false;

    /// <summary>
    /// Specifies whether the cookie should be restricted to first-party
    /// or same-site usage. It can have three values: Strict, Lax, or None.
    /// </summary>
    public SameSiteType SameSite { get; set; } = SameSiteType.Lax;

    /// <summary>Serializes a cookie object from a string</summary>
    /// <param name="cookie">The raw string to convert.</param>
    /// <param name="decode">Whether to lazily UriDecode the cookie.</param>
    /// <returns>A parsed cookie instance.</returns>
    /// <exception cref="ConstraintException">
    /// The raw string doesn't follow the RFC 2625 specification for cookies.
    /// </exception>
    public static Cookie Parse(string cookie, bool decode)
    {
        if (decode)
            cookie = UriHelpers.DecodeUriComponent(cookie);
        
        string[] split = cookie.Split(';')
            .Select(v => v.Trim())
            .ToArray();
        
        if (!split[0].Contains('='))
            throw new ConstraintException("Cookies must start with the name and value.");

        string[] nameAndValue = split[0].Split('=');

        Cookie instance = new()
        {
            Name = nameAndValue[0],
            Value = string.Join('=', nameAndValue[1..]),
            HttpOnly = split.Contains("HttpOnly"),
            Secure = split.Contains("Secure")
        };

        string? domain = null;
        SetIfExists("Domain", ref domain);
        instance.Domain = domain;

        string? path = null;
        SetIfExists("Path", ref path);
        instance.Path = path;

        return instance;

        void SetIfExists(string toFind, ref string? toSet)
        {
            string? found = split.FirstOrDefault(s => s.StartsWith(toFind));

            if (found is null)
                return;

            string[] splitFound = found.Split('=');

            if (splitFound.Length != 2)
                throw new ConstraintException($"Invalid value for {toFind} in a cookie.");

            toSet = split[1];
        }
    }
    
    /// <inheritdoc cref="Object.ToString"/>
    public override string ToString()
    {
        string result = $"""
                         {Name}={Value}; 
                         Expires={Expires.ToUniversalTime()};
                         {(Secure ? "Secure;" : string.Empty)}
                         {(HttpOnly ? "HttpOnly;": string.Empty)}
                         SameSite={SameSite.ToString()}; 
                         """.Replace("\n", string.Empty);

        if (Domain is not null)
            result += $"Domain={Domain};";

        if (Path is not null)
            result += $"Path={Path};";
        
        return result[..^1]
            .Replace(";", "; ");
    }
}