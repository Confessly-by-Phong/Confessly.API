using Microsoft.AspNetCore.Http;

namespace Confessly.Contracts.Authentication
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public Guid GetCurrentUserId() => _httpContextAccessor.HttpContext?.User?.GetUserId() ?? Guid.Empty;
    }
}
