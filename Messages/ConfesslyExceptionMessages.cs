namespace Confessly.Messages
{
    public static class ConfesslyExceptionMessages
    {
        public const string ValidationFailed = "Validation failed: {0}";
        public const string InternalServerError = "An unexpected error occurred. Please try again later. Details: {0}";

        public static string ConfesslyMessage(this Exception exception)
        {
            var message = exception.Message;
            if (exception.InnerException is not null)
            {
                message += $" Inner message: {exception.InnerException.Message}";
            }
            return message;
        }
    }
}
