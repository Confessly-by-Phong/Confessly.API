using System.Security.Claims;

namespace Confessly.Contracts.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            string? userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userId, out var parsedUserId)) return parsedUserId;
            return Guid.Empty;
        }
    }
}
