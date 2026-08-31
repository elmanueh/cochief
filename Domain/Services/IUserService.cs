using Cochief.Domain.Model;

namespace Cochief.Domain.Services;

public interface IUserService
{
    public User CreateUser(string name, string email, string password);
    public User GetUser(Guid userId);
    public Task<User> LinkPlayerAsync(Guid userId, string playerTag, string token);
    public void UnlinkPlayer(Guid userId);
    public User GetUserByEmail(string email);
}
