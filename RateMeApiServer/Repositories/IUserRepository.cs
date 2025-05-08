using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
    }
}
