using System.Collections.ObjectModel;
using RateMe.Api.Clients;
using RateMe.Api.Mappers;
using RateMe.Models.ClientModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;
using RateMe.Services.Interfaces;
using RateMeShared.Dto;

namespace RateMe.Services;

public class SubjectsService : ILocalSubjectsService, ISubjectUpdater
{
    public SubjectsClient? SubjClient { get; set; }
    public bool IsAnyData => _allSubjects.Count != 0;
    public bool IsRemoteAlive { get; }
    
    private readonly ObservableCollection<Subject> _allSubjects;
    
    private HashSet<SubjectLocal> _subjectsToUpdate = [];
    private List<int> _subjKeysToRemove = [];
    
    private SubjectsRepository _rep = new();

    
    public SubjectsService(ObservableCollection<Subject> allSubjects, bool isRemoteAlive)
    {
        _allSubjects = allSubjects;
        IsRemoteAlive = isRemoteAlive;
    }
    
    
    public async Task SubjectsOverallRemoteUpdate()
    {
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

        bool serverVisited = subjectsToAdd.Length != 0 || _subjectsToUpdate.Count != 0 || _subjKeysToRemove.Count != 0;
        
        if (serverVisited) // If server visited no exceptions, everything is up to date
        {
            await MarkAllUpToDate(); 
        }
    }
    
    
    /// <summary>
    /// Pushes given subjects to remote bd
    /// </summary>
    private async Task PushSubjects(SubjectLocal[] subjectsToAdd)
    {
        SubjectDto[] subjDtos = subjectsToAdd.Select(SubjectMapper.GetSubjectDto).ToArray(); 
        
        List<SubjectId>? subjIds = await SubjClient!.PushSubjects(subjDtos);
        
        // Updating remote keys if success
        if (subjIds != null)
        {
            await _rep.UpdateRemoteKeys(subjIds);
        }
    }

    
    /// <summary>
    /// Requests update of subjects
    /// </summary>
    private async Task UpdateSubjectsRemote(IEnumerable<SubjectLocal> subjects) // TODO: если remote не работал, то update'a не будет, можно локально добавить колонку 'saved'  
    {
        List<PlainSubject> subjsDto = [];
        
        foreach (SubjectLocal subj in subjects)
        {
            PlainSubject dto = new() { RemoteId = subj.RemoteId, Name = subj.Name, Credits = subj.Credits };
            subjsDto.Add(dto);
        }

        await SubjClient!.UpdateSubjects(subjsDto);
    }

    
    /// <summary>
    /// Requests removal of subjects
    /// </summary>
    private async Task RemoveSubjectsByKeysRemote(List<int> subjectsKeys)
    {
        await SubjClient!.RemoveSubjectsByKeys(subjectsKeys);
    }

    /// <summary>
    /// Gets all user's subjects from remote and saves locally 
    /// </summary>
    public async Task LoadUpdateAllUserSubjectsFromRemote()
    {
        SubjectDto[]? subjDtos = await SubjClient!.GetAllSubjects();

        if (subjDtos == null)
        {
            return;
        }

        SubjectLocal[] subjectsToAdd = subjDtos.Select(SubjectMapper.GetLocalFromDto).ToArray();
        await _rep.Add(subjectsToAdd);
        
        IEnumerable<SubjectLocal> oldSubjects = _allSubjects.Select(s => s.LocalModel);
        await _rep.Remove(oldSubjects);
        
        UpdateObservable(subjectsToAdd);
    }
    
    
    /// <summary>
    /// Catches subjects to be updated, before they are saved to local bd
    /// </summary>
    public void RetainSubjectsToUpdate()
    {
        foreach (Subject subj in _allSubjects)
        {
            if (subj.LocalModel.RemoteId == 0)
            {
                continue;
            }

            bool isToUpdate = subj.LocalModel.RemoteStatus == RemoteStatus.ToUpdate;
            
            if (isToUpdate || subj.Name != subj.LocalModel.Name || subj.Credits != subj.LocalModel.Credits)
            {
                _subjectsToUpdate.Add(subj.LocalModel);
            }
        }
    }

    // Local Stuff

    /// <summary>
    /// In case of not working server, marks changes so that next time they were pushed to server.
    /// Rn marks only ToUpdate, ToAdd implemented earlier via RemoteId == 0, ToRemove - I'm lazy to implement ;) 
    /// </summary>
    public async Task MarkRemoteStates()
    {
        HashSet<int> idsToUpdate = _subjectsToUpdate.Select(s => s.SubjectId).ToHashSet();
        await _rep.MarkToUpdate(idsToUpdate);
    }
    

    public async Task<List<SubjectLocal>> GetAllLocals()
    {
        return await _rep.GetAll();
    }


    public async Task UpdateAllLocals()
    {
        foreach (Subject subject in _allSubjects)
        {
            subject.UpdateLocalModel();
        }

        SubjectLocal[] locals = _allSubjects.Select(c => c.LocalModel).ToArray();
        await _rep.Update(locals);
    }


    public async Task AddLocals(IEnumerable<Subject> subjs)
    {
        SubjectLocal[] locals = subjs.Select(c => c.LocalModel).ToArray();
        await _rep.Add(locals);
    }


    public async Task AddLocal(SubjectLocal subj)
    {
        await _rep.Add(subj);
    }

    
    /// <summary>
    /// Removes all subjects locally. Only locally
    /// </summary>
    public async Task ClearLocal()
    {
        await RemoveLocals(_allSubjects);
        _allSubjects.Clear();
    }

    
    public async Task RemoveLocal(SubjectLocal subj)
    {
        if (subj.RemoteId != 0)
        {
            _subjKeysToRemove.Add(subj.RemoteId);
        }

        await _rep.Remove(subj);
    }
    
    
    public async Task RemoveLocals(IEnumerable<Subject> subjs)
    {
        SubjectLocal[] locals = subjs.Select(c => c.LocalModel).ToArray();
        await _rep.Remove(locals);
    }

    private void UpdateObservable(IEnumerable<SubjectLocal> newSubs)
    {
        _allSubjects.Clear();

        foreach (SubjectLocal subLocal in newSubs)
        {
            Subject subj = new(subLocal);
            _allSubjects.Add(subj);
        }
    }
    
    private async Task MarkAllUpToDate()
    {
        await _rep.MarkUpToDate();
    }
}