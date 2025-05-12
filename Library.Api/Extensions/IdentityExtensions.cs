using System.IdentityModel.Tokens.Jwt;

namespace Library.Api.Extensions;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userId = context.User.Claims
                         .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub) ??
                     context.User.Claims
                         .FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userId?.Value, out var parsedId))
        {
            return parsedId;
        }

        return null;
    }
}