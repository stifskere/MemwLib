namespace MemwLib.Http.Extensions.Cookies.Types;

/// <summary>Valid values for the SamSite attribute in a cookie.</summary>
public enum SameSiteType
{
    /// <summary>
    /// Lax is the default if SameSite isn't specified. Previously,
    /// cookies were sent for all requests by default. 
    /// </summary>
    Strict,
    
    /// <summary>
    /// Cookies with SameSite=None must now also specify the Secure
    /// attribute (they require a secure context
    /// </summary>
    Lax,
    
    /// <summary>
    /// Cookies from the same domain are no longer considered to be
    /// from the same site if sent using a different scheme (http: or https:).
    /// </summary>
    None
}