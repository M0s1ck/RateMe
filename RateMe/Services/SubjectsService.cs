using Microsoft.EntityFrameworkCore;
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
    public List<int> SubjKeysToRemove { get; } = [];
    
    private readonly IEnumerable<Subject> _allSubjects;
    private List<SubjectLocal> _subjectsToUpdate = [];
    private Dictionary<int, SubjectLocal> _subjectsToAdd = [];
    
    private SubjectsRepository _rep = new();
    private SubjectsClient _subjClient = new();

    
    public SubjectsService(IEnumerable<Subject> allSubjects)
    {
        _allSubjects = allSubjects;
    }
    
    
    public async Task SubjectsOverallRemoteUpdate()
    {
        _subjectsToAdd = await _rep.GetSubjectsNoRemote();
        
        if (_subjectsToAdd.Count != 0)
        {
            int userId = JsonModelsHandler.GetUserId();
            await PushSubjectsByUserId(userId, _subjectsToAdd);   // TODO: refactor for not working server
        }
        
        if (_subjectsToUpdate.Count != 0)
        {
            await UpdateSubjectsRemote(_subjectsToUpdate);
        }

        if (SubjKeysToRemove.Count != 0)
        {
            await RemoveSubjectsByKeysRemote(SubjKeysToRemove);
        }
    }
    
    
    /// <summary>
    /// Pushes ALL local subjects to remote bd
    /// </summary>
    public async Task PushAllSubjectsByUserId(int userId)
    {
        // Building up Dto
        SubjectsByUserId subjectsObj = new();
        subjectsObj.UserId = userId;

        await using SubjectsContext context = new();
        
        foreach (SubjectLocal subj in context.Subjects.Include(s => s.Elements)) // Takes all of them!!
        {
            SubjectDto subjDto = SubjectMapper.GetSubjectDto(subj);    
            subjectsObj.Subjects.Add(subjDto);
        }
        
        // Pushing
        List<SubjectId>? subjIds = await _subjClient.PushSubjectsByUserId(subjectsObj);
        
        // Updating remote keys if success
        if (subjIds != null)
        {
            await _rep.UpdateRemoteKeys(subjIds);
        }
    }
    
    
    /// <summary>
    /// Pushes given subjects to remote bd
    /// </summary>
    private async Task PushSubjectsByUserId(int userId, Dictionary<int, SubjectLocal> subjectsToAdd)
    {
        SubjectsByUserId subjectsObj = new();
        subjectsObj.UserId = userId;

        foreach ((int _, SubjectLocal subj) in subjectsToAdd)
        {
            SubjectDto subjDto = SubjectMapper.GetSubjectDto(subj);    
            subjectsObj.Subjects.Add(subjDto);
        }
        
        List<SubjectId>? subjIds = await _subjClient.PushSubjectsByUserId(subjectsObj);
        
        // Updating remote keys if success
        if (subjIds != null)
        {
            await _rep.UpdateRemoteKeys(subjIds);
        }
    }

    
    /// <summary>
    /// Requests update of subjects
    /// </summary>
    private async Task UpdateSubjectsRemote(List<SubjectLocal> subjects)
    {
        List<PlainSubject> subjsDto = [];
        
        foreach (SubjectLocal subj in subjects)
        {
            PlainSubject dto = new() { RemoteId = subj.RemoteId, Name = subj.Name, Credits = subj.Credits };
            subjsDto.Add(dto);
        }

        await _subjClient.UpdateSubjects(subjsDto);
    }

    
    /// <summary>
    /// Requests removal of subjects
    /// </summary>
    private async Task RemoveSubjectsByKeysRemote(List<int> subjectsKeys)
    {
        await _subjClient.RemoveSubjectsByKeys(subjectsKeys);
    }

    
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
            SubjKeysToRemove.Add(subj.RemoteId);
        }

        await _rep.Remove(subj);
    }


    internal async Task RemoveLocals(IEnumerable<Subject> subjs)
    {
        SubjectLocal[] locals = subjs.Select(c => c.LocalModel).ToArray();
        await _rep.Remove(locals);
    }
    
    
    /// <summary>
    /// Catches subjects to be updated, before they are saved to local bd
    /// </summary>
    internal void RetainSubjectsToUpdate()
    {
        foreach (Subject subj in _allSubjects)
        {
            bool isDiff = subj.Name != subj.LocalModel.Name || subj.Credits != subj.LocalModel.Credits;
            bool isToAdd = _subjectsToAdd.ContainsKey(subj.LocalModel.SubjectId);
            
            if (isDiff && !isToAdd)
            {
                _subjectsToUpdate.Add(subj.LocalModel);
            }
        }
    }
}