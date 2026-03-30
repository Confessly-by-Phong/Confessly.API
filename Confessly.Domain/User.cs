using Confessly.Domain.Core;

namespace Confessly.Domain
{
    public class User : BaseEntity, IUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public User() { }

        public User(IUser user)
        {
            Username = user.Username;
            Password = user.Password;
            Name = user.Name;
        }
    }
}