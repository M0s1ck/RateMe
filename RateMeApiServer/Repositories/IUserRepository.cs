using RateMeApiServer.Common;
using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Repositories
{
    public interface IUserRepository
    {
        Task<DbInteractionResult<User>> GetByIdAsync(int id);
        Task<DbInteractionResult<int>> AddAsync(User user);
        Task<DbInteractionResult<User>> AuthAsync(string email, string password);
        Task<DbInteractionStatus> UpdateAsync(User user);
        Task<DbInteractionStatus> RemoveAsync(int id);
    }
}
