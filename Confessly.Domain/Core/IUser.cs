namespace Confessly.Domain.Core
{
    public interface IUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
}
