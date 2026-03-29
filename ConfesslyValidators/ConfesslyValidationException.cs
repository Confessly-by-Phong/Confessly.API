using Confessly.Messages;

namespace Confessly.Validators
{
    public class ConfesslyValidationException : Exception
    {
        public ConfesslyValidationException(string message)
            : base(string.Format(ConfesslyExceptionMessages.ValidationFailed, message))
        {
        }

        public ConfesslyValidationException(string messageTemplate,
            params object[] messageArgs)
            : base(string.Format(ConfesslyExceptionMessages.ValidationFailed,
                string.Format(messageTemplate, messageArgs)))
        {
        }
    }
}
