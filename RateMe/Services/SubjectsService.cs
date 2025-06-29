using RateMe.Api.Clients;
using RateMe.Api.Mappers;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;
using RateMeShared.Dto;

namespace RateMe.Services;

public class SubjectsService
{
    private readonly IEnumerable<Subject> _allSubjects;
    
    private List<SubjectLocal> _subjectsToUpdate = [];
    private List<int> _subjKeysToRemove = [];
    
    private SubjectsRepository _rep = new();
    private SubjectsClient? _subjClient;

    
    public SubjectsService(IEnumerable<Subject> allSubjects)
    {
        _allSubjects = allSubjects;
    }
    
    
    public async Task SubjectsOverallRemoteUpdate()
    {
        int userId = JsonModelsHandler.GetUserId(); // TODO: what if no user??
        _subjClient = new SubjectsClient(userId);
        
        SubjectLocal[] subjectsToAdd = await _rep.GetSubjectsNoRemote();
        
        if (subjectsToAdd.Length != 0)
        {
            await PushSubjects(subjectsToAdd);   // TODO: refactor for not working server
        }
        
        if (_subjectsToUpdate.Count != 0)
        {
            await UpdateSubjectsRemote(_subjectsToUpdate);
        }

        if (_subjKeysToRemove.Count != 0)
        {
            await RemoveSubjectsByKeysRemote(_subjKeysToRemove);
        }
    }
    
    
    /// <summary>
    /// Pushes given subjects to remote bd
    /// </summary>
    private async Task PushSubjects(SubjectLocal[] subjectsToAdd)
    {
        SubjectDto[] subjDtos = subjectsToAdd.Select(SubjectMapper.GetSubjectDto).ToArray(); 
        
        List<SubjectId>? subjIds = await _subjClient!.PushSubjects(subjDtos);
        
        // Updating remote keys if success
        if (subjIds != null)
        {
            await _rep.UpdateRemoteKeys(subjIds);
        }
    }

    
    /// <summary>
    /// Requests update of subjects
    /// </summary>
    private async Task UpdateSubjectsRemote(List<SubjectLocal> subjects) // TODO: если remote не работал, то update'a не будет, можно локально добавить колонку 'saved'  
    {
        List<PlainSubject> subjsDto = [];
        
        foreach (SubjectLocal subj in subjects)
        {
            PlainSubject dto = new() { RemoteId = subj.RemoteId, Name = subj.Name, Credits = subj.Credits };
            subjsDto.Add(dto);
        }

        await _subjClient!.UpdateSubjects(subjsDto);
    }

    
    /// <summary>
    /// Requests removal of subjects
    /// </summary>
    private async Task RemoveSubjectsByKeysRemote(List<int> subjectsKeys)
    {
        await _subjClient!.RemoveSubjectsByKeys(subjectsKeys);
    }
    
    
    /// <summary>
    /// Catches subjects to be updated, before they are saved to local bd
    /// </summary>
    internal void RetainSubjectsToUpdate()
    {
        foreach (Subject subj in _allSubjects)
        {
            if (subj.LocalModel.RemoteId == 0)
            {
                continue;
            }
            
            if (subj.Name != subj.LocalModel.Name || subj.Credits != subj.LocalModel.Credits)
            {
                _subjectsToUpdate.Add(subj.LocalModel);
            }
        }
    }

    // Local Stuff
    
    internal async Task<List<SubjectLocal>> GetAllLocals()
    {
        return await _rep.GetAll();
    }

    
    internal async Task UpdateAllLocals()
    {
        foreach (Subject subject in _allSubjects)
        {
            subject.UpdateLocalModel();
        }

        SubjectLocal[] locals = _allSubjects.Select(c => c.LocalModel).ToArray();
        
        await _rep.Update(locals);
    }

    
    internal async Task AddLocals(List<Subject> subjs)
    {
        SubjectLocal[] locals = subjs.Select(c => c.LocalModel).ToArray();
        await _rep.Add(locals);
    }
    
    
    internal async Task AddLocal(SubjectLocal subj)
    {
        await _rep.Add(subj);
    }
    
    
    internal async Task RemoveLocal(SubjectLocal subj)
    {
        if (subj.RemoteId != 0)
        {
            _subjKeysToRemove.Add(subj.RemoteId);
        }

        await _rep.Remove(subj);
    }


    internal async Task RemoveLocals(IEnumerable<Subject> subjs)
    {
        SubjectLocal[] locals = subjs.Select(c => c.LocalModel).ToArray();
        await _rep.Remove(locals);
    }
}