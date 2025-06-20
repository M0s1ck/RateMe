using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Repositories;

public interface ISubjectRepository
{
    public Task<List<Subject>> AddSubjectsAsync(int userId, List<Subject> subjects);
    public Task RemoveSubjectsByKeys(List<int> keys);
}