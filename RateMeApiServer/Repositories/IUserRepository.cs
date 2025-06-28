using RateMeApiServer.Common;
using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Repositories
{
    public interface IUserRepository
    {
        Task<DbInteractionResult<User>> GetByIdAsync(int id);
        Task<DbInteractionResult<int>> AddAsync(User user);
        Task<DbInteractionResult<int>> AuthAsync(string email, string password);
    }
}
