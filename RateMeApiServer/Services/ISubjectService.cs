using RateMeShared.Dto;

namespace RateMeApiServer.Services;

public interface ISubjectService
{
    public Task<SubjectsIds> AddSubjectsAsync(SubjectsByUserId subjects);
}