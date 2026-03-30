using Confessly.Contracts.Authentication;
using Confessly.Domain;
using Confessly.Infrastructure.Authentication;
using Confessly.Logging.Interfaces;
using Confessly.Messages;
using Confessly.Repository.Core;
using Confessly.Services.Core;
using Confessly.Services.Validation;
using Confessly.Validators;
using Microsoft.AspNetCore.Identity;

namespace Confessly.Services
{
    public class AuthenticationServices : BaseServices, IAuthenticationServices
    {
        private IConfesslyAuthentication _authenticator;

        public AuthenticationServices(IUnitOfWork work, ILoggingService logger,
            IConfesslyAuthentication authenticator) : base(work, logger)
        {
            _authenticator = authenticator;
        }

        public async Task<string> AuthenticateUser(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _work.Users.Get(u => u.Username == username, cancellationToken: cancellationToken);
            if (user is null || !_authenticator.VerifyPassword(user, password))
            {
                throw new ConfesslyValidationException(ConfesslyValidationMessages.UsernameOrPasswordInvalid);
            }

            return _authenticator.GenerateJwtToken(user);
        }

        public async Task<User> CreateUser(UserCreate userCreate, CancellationToken cancellationToken)
        {
            var user = userCreate.ToUser();

            await user.Validate(_work.Users, cancellationToken);
            user.Password = _authenticator.HashPassword(user);
            var createdUser = await _work.Users.Insert(user, cancellationToken);
            await _work.SaveChanges(cancellationToken);
            createdUser.Password = string.Empty;

            return createdUser;
        }
    }
}
