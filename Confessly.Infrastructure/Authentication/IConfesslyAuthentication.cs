using Confessly.Domain;

namespace Confessly.Infrastructure.Authentication
{
    public interface IConfesslyAuthentication
    {
        string HashPassword(User user);
        bool VerifyPassword(User user, string passwordToVerify);
        string GenerateJwtToken(User user);
    }
}
