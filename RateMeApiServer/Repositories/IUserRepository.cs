using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<int> AddAsync(User user);
        Task<User> AuthAsync(string email, string password);
    }
}
