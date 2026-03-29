using Confessly.Domain;
using Confessly.Messages;
using Confessly.Validators;
using ConfesslyValidators;

namespace Confessly.Services.Validation
{
    public static class UserValidator
    {
        public static void Validate(this User user)
        {
            if (user is null)
                throw new ConfesslyValidationException(ConfesslyValidationMessages.UserCannotBeNull);

            user.Username.StringValidate(nameof(user.Username), false, 5, 50);
            user.Password.StringValidate(nameof(user.Password), false);
        }
    }
}
