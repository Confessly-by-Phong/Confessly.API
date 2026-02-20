using Confessly.Domain;
using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;
using Confessly.Services.Core;
using Confessly.Services.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Services
{
    public class UserServices : BaseServices
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public UserServices(IUnitOfWork work, ILoggingService logger) : base(work, logger)
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<User> CreateUser(User user, CancellationToken cancellationToken)
        {
            user.Validate();
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
