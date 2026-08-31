using Cochief.Domain.Model;

namespace Cochief.Domain.Services;

public interface IAuthService
{
    public User Register(string name, string email, string password);
    public User Login(string email, string password);
}
