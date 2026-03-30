using Confessly.Domain;
using Confessly.Domain.Core;

namespace Confessly.Contracts.Authentication
{
    public class UserCreate : IUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public User ToUser()
        {
            var user = new User
            {
                Username = this.Username,
                Password = this.Password,
                Name = this.Name
            };

            if (string.IsNullOrWhiteSpace(user.Name))
                user.Name = user.Username; // Default to username if name is not provided

            return user;
        }
    }
}
