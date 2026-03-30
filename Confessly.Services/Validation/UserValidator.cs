using Confessly.Domain;
using Confessly.Domain.Core;
using Confessly.Messages;
using Confessly.Repository.Core;
using Confessly.Validators;

namespace Confessly.Services.Validation
{
    public static class UserValidator
    {
        public static async Task Validate(this User user, IRepository<User> repository)
        {
            if (user is null)
                throw new ConfesslyValidationException(ConfesslyValidationMessages.UserCannotBeNull);

            user.Username.StringValidate(nameof(user.Username), false, 5, 50);
            user.Password.StringValidate(nameof(user.Password), false, 10);

            var existingUser = await repository.Get(u => u.Username.ToLower() == user.Username.ToLower());
            if (existingUser is not null)
                throw new ConfesslyValidationException(ConfesslyValidationMessages.UsernameAlreadyTakenMessage(user.Username));
        }
    }
}
