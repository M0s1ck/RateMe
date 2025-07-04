using RateMeApiServer.Common;
using RateMeShared.Dto;

namespace RateMeApiServer.Services;

public interface ISubjectService
{
    public Task<DbInteractionResult<IEnumerable<SubjectDto>>> GetAllSubjectsAsync(int userId);
    public Task<DbInteractionResult<SubjectsIds>> AddSubjectsAsync(int userId, IEnumerable<SubjectDto> subjects);
    public Task<DbInteractionStatus> UpdateSubjectsAsync(int userId, IEnumerable<PlainSubject> subjects);
    public Task<DbInteractionStatus> RemoveSubjectsAsync(int userId, IEnumerable<int> plainKeysObj);
}