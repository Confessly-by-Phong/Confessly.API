using Confessly.Domain;
using Confessly.Domain.Core;
using System.ComponentModel.DataAnnotations;

namespace Confessly.Contracts.Authentication
{
    public class UserCreate : IUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;

        public User ToUser() => new User(this);
    }
}
