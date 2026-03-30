namespace Confessly.Messages
{
    public static partial class ConfesslyValidationMessages
    {
        public const string UserCannotBeNull = "User cannot be null.";
        public const string InvalidUsernameLength = "Username must be between 5 and 50 characters.";
        public const string PasswordCannotBeEmpty = "Password cannot be empty.";

        private const string UsernameAlreadyTaken = "Username '{0}' is already taken.";

        public static string UsernameAlreadyTakenMessage(string username)
        {
            return string.Format(UsernameAlreadyTaken, username);
        }
    }
}
