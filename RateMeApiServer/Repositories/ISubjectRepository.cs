using RateMeApiServer.Common;
using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Repositories;

public interface ISubjectRepository
{
    /// <summary>
    /// Adds subjects to user with userId
    /// </summary>
    /// <returns>Subjects with updated keys or null if user was not found</returns>
    public Task<DbInteractionResult<IEnumerable<Subject>>> AddSubjects(int userId, IEnumerable<Subject> subjects);
    public Task<DbInteractionStatus> UpdateSubjects(int userId, IEnumerable<PlainSubject> subjects);
    public Task<DbInteractionStatus> RemoveSubjectsByKeys(int userId, IEnumerable<int> keys);
}