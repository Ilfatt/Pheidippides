using System.Globalization;
using System.Security.Claims;
using Pheidippides.Domain;

namespace Pheidippides.Api.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Получить id пользователя из httpContext
    /// </summary>
    /// <param name="httpContext">HttpContext</param>
    /// <returns>id пользователя</returns>
    /// <exception cref="ArgumentException">Пользователь не авторизирован или нет клэйма
    /// с типом ClaimTypes.NameIdentifier</exception>
    public static long GetUserId(this HttpContext? httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.User.Identity is { IsAuthenticated: false })
            throw new ArgumentException("User not authenticated");

        var claim = httpContext.User.Claims
                        .FirstOrDefault(claim => claim.Type == nameof(ClaimTypes.NameIdentifier))
                    ?? throw new ArgumentException($"Authenticated user has not claim" +
                                                   $" with type '{nameof(ClaimTypes.NameIdentifier)}'");

        return long.Parse(claim.Value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Метод для получения UserRole из httpContext
    /// </summary>
    /// <param name="httpContext">HttpContext</param>
    /// <returns>UserRole</returns>
    /// <exception cref="ArgumentException">Пользователь не авторизирован или нет клэйма
    /// с типом ClaimTypes.Role</exception>
    public static UserRole GetUserRole(this HttpContext? httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.User.Identity is { IsAuthenticated: false })
            throw new ArgumentException("User not authenticated");

        var claim = httpContext.User.Claims
                        .First(claim => claim.Type == nameof(ClaimTypes.Role))
                    ?? throw new ArgumentException($"Authenticated user has not claim" +
                                                   $" with type '{nameof(ClaimTypes.Role)}'");

        return (UserRole)int.Parse(claim.Value, CultureInfo.InvariantCulture);
    }
}