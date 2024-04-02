using MemwLib.Http.Extensions.Cookies.Types;
using MemwLib.Http.Types.Entities;

namespace MemwLib.Http.Extensions.Cookies;

/// <summary>
/// This class contains extension methods for HTTP
/// entities to manage cookies within them.
/// </summary>
public static class CookieManager
{
    /// <summary>
    /// Add or replace a cookie to this responsible entity such as middleware or response entity.
    /// </summary>
    /// <param name="entity">The entity that's going the be assigned thus cookie.</param>
    /// <param name="cookie">The cookie to assign to thus entity.</param>
    /// <typeparam name="TEntity">The type of the target, must implement IResponsible</typeparam>
    /// <returns>The same instance to act as a constructor.</returns>
    public static TEntity WithCookie<TEntity>(this TEntity entity, Cookie cookie) where TEntity : IResponsible
    {
        
        
        return entity;
    }

    public static TEntity WithoutCookie<TEntity>(this TEntity entity, string name) where TEntity : IResponsible
    {
        return entity;
    }
}