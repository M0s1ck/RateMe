using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Repositories;

public interface ISubjectRepository
{
    public Task<List<Subject>> AddSubjectsAsync(int userId, List<Subject> subjects);
}