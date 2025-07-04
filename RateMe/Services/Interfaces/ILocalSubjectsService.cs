using RateMe.Models.ClientModels;
using RateMe.Models.LocalDbModels;

namespace RateMe.Services.Interfaces;

public interface ILocalSubjectsService
{
    Task<List<SubjectLocal>> GetAllLocals();
    Task UpdateAllLocals();
    Task AddLocals(IEnumerable<Subject> subjs);
    Task AddLocal(SubjectLocal subj);
    Task RemoveLocal(SubjectLocal subj);
    Task RemoveLocals(IEnumerable<Subject> subjs);
}