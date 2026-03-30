using Confessly.Contracts.Authentication;
using Confessly.Domain;

namespace Confessly.Services.Core
{
    public interface IAuthenticationServices
    {
        Task<User> CreateUser(UserCreate userCreate, CancellationToken cancellationToken);
        Task<string> AuthenticateUser(string email, string password, CancellationToken cancellationToken);
    }
}
