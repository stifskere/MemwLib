using MemwLib.Http.Extensions.Cookies.Exceptions;
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
    /// <exception cref="CookieConstraintException">Thrown when the cookie date isn't future.</exception>
    /// <returns>The same instance to act as a constructor.</returns>
    public static TEntity WithCookie<TEntity>(this TEntity entity, Cookie cookie) where TEntity : IResponsible
    {
        if (cookie.Expires < DateTime.Now)
            throw new CookieConstraintException(
                $"You can't set a cookie with anterior date, please use the {nameof(WithoutCookie)} method."
            );
        
        return (TEntity)entity
            .WithHeader("Set-Cookie", cookie.ToString());
    }

    /// <summary>Effectively removes a cookie from the client the response is going to be sent to.</summary>
    /// <param name="entity">The response entity that's going to be sent to the client.</param>
    /// <param name="name">The name of the cookie to remove.</param>
    /// <typeparam name="TEntity">The type of target, must implement IResponsible</typeparam>
    /// <returns>The same instance with the specific header </returns>
    public static TEntity WithoutCookie<TEntity>(this TEntity entity, string name) where TEntity : IResponsible
    {
        return (TEntity)entity
            .WithHeader(
                "Set-Cookie", 
                new Cookie
                {
                    Name = name, 
                    Value = string.Empty, 
                    Expires = DateTime.MinValue
                }
            );
    }
}