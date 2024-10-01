using POECLDPart1.Models;

namespace POECLDPart1.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
    }
}
