using Confessly.Domain;
using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;
using Confessly.Services.Core;
using Confessly.Services.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Services
{
    public class UserServices(IUnitOfWork work, ILoggingService logger) : BaseServices(work, logger)
    {
        public async Task<User> CreateUser(User user, CancellationToken cancellationToken)
        {
            user.Validate();
            var createdUser = await _work.Users.Insert(user, cancellationToken);
            await _work.SaveChanges(cancellationToken);
            createdUser.Password = string.Empty;

            return createdUser;
        }
    }
}
