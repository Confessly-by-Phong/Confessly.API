using Confessly.Contracts.Authentication;
using Confessly.Domain;
using Confessly.Domain.Core;
using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;
using Confessly.Services.Core;
using Confessly.Services.Validation;
using Microsoft.AspNetCore.Identity;

namespace Confessly.Services
{
    public class UserServices : BaseServices
    {
        private readonly PasswordHasher<IUser> _passwordHasher;

        public UserServices(IUnitOfWork work, ILoggingService logger) : base(work, logger)
        {
            _passwordHasher = new PasswordHasher<IUser>();
        }

        public async Task<User> CreateUser(UserCreate userCreate, CancellationToken cancellationToken)
        {
            var user = userCreate.ToUser();

            await user.Validate(_work.Users);
            user.Password = HashPassword(user);
            var createdUser = await _work.Users.Insert(user, cancellationToken);
            await _work.SaveChanges(cancellationToken);
            createdUser.Password = string.Empty;

            return createdUser;
        }

        private string HashPassword(User user)
        {
            return _passwordHasher.HashPassword(user, user.Password);
        }

        private bool VerifyPassword(User user, string encryptedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, encryptedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
