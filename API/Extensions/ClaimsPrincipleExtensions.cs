using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            // fetches the user's username from the token that the api is using to authenticate this endpoint
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}