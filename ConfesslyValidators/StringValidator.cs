using Confessly.Messages;
using Confessly.Validators;

namespace ConfesslyValidators
{
    public static class StringValidator
    {
        /// <summary>
        /// Validate a string
        /// </summary>
        /// <param name="stringToValidate">String to Validate</param>
        /// <param name="propertyName">Name of the property to be validated, used in the error message.</param>
        /// <param name="allowEmpty">True if the string is allowed to be empty. Default is False</param>
        /// <param name="minLength">Minimum length of the string. Default is -1, which means that the string is not required to be validated in the minimum length</param>
        /// <param name="maxLength">Maximum length of the string, Default is -1, which means that the string is not required to be validated in the maximum length</param>
        /// <returns>True if the string meets all the validation, otherwise, throw Exception with message of the corresponding message</returns>
        /// <exception cref="ConfesslyValidationException">Throw exception with the error message if the string is not valid</exception>
        public static bool StringValidate(this string stringToValidate,
            string propertyName,
            bool allowEmpty = false,
            int minLength = -1,
            int maxLength = -1)
        {
            if (string.IsNullOrEmpty(stringToValidate))
            {
                if (allowEmpty) return true;

                throw new ConfesslyValidationException(
                    ConfesslyValidationMessages.EmptyProperty, propertyName);
            }

            if (minLength > 0 && stringToValidate.Length < minLength)
                throw new ConfesslyValidationException(
                    ConfesslyValidationMessages.MinLength, propertyName, minLength);

            if (maxLength > 0 && stringToValidate.Length > maxLength)
                throw new ConfesslyValidationException(
                    ConfesslyValidationMessages.MaxLength, propertyName, maxLength);

            return true;
        }
    }
}
