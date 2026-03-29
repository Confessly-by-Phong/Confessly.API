namespace Confessly.Contracts.Authentication
{
    public interface IUserContext
    {
        Guid GetCurrentUserId();
    }
}
