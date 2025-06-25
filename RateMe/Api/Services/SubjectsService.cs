using Microsoft.EntityFrameworkCore;
using RateMe.Api.Clients;
using RateMe.Api.Mappers;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;
using RateMeShared.Dto;

namespace RateMe.Api.Services;

public class SubjectsService
{
    public Dictionary<int, SubjectLocal> SubjectsToAdd { get; private set; } = [];
    public List<int> SubjKeysToRemove { get; } = [];
    
    private IEnumerable<Subject> _allSubjects;
    private List<Subject> _subjectsToUpdate = [];
    
    private SubjectsClient _subjClient;
    private SubjectsRepository _rep = new();

    public SubjectsService(IEnumerable<Subject> allSubjects, SubjectsClient subjClient)
    {
        _allSubjects = allSubjects;
        _subjClient = subjClient;
    }
    
    public async Task SubjectsOverallRemoteUpdate()
    {
        if (SubjectsToAdd.Count != 0)
        {
            int userId = JsonModelsHandler.GetUserId();
            await PushSubjectsByUserId(userId, SubjectsToAdd);   // TODO: refactor for not working server
        }
        
        if (_subjectsToUpdate.Count != 0)
        {
            await UpdateSubjects(_subjectsToUpdate);
        }

        if (SubjKeysToRemove.Count != 0)
        {
            await RemoveSubjectsByKeys(SubjKeysToRemove);
        }
    }
    
    /// <summary>
    /// Pushes ALL local subjects to remote bd
    /// </summary>
    /// <param name="userId"></param>
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

    private async Task UpdateSubjects(List<Subject> subjects)
    {
        List<PlainSubject> subjsDto = [];
        
        foreach (Subject subj in subjects)
        {
            PlainSubject dto = new() { RemoteId = subj.LocalModel.RemoteId, Name = subj.Name, Credits = subj.Credits };
            subjsDto.Add(dto);
        }

        await _subjClient.UpdateSubjects(subjsDto);
    }

    /// <summary>
    /// Requests removal of subjects
    /// </summary>
    private async Task RemoveSubjectsByKeys(List<int> subjectsKeys)
    {
        await _subjClient.RemoveSubjectsByKeys(subjectsKeys);
    }
    
    /// <summary>
    /// Catches subjects to updates before they are saved to local bd
    /// </summary>
    internal void RetainSubjectsToUpdate()
    {
        foreach (Subject subj in _allSubjects)
        {
            bool isDiff = subj.Name != subj.LocalModel.Name || subj.Credits != subj.LocalModel.Credits;
            bool isToAdd = SubjectsToAdd.ContainsKey(subj.LocalModel.SubjectId);
            
            if (isDiff && !isToAdd)
            {
                _subjectsToUpdate.Add(subj);
            }
        }
    }

    internal async Task SetSubjectsNoRemoteToAdd()
    {
        SubjectsToAdd = await _rep.GetSubjectsNoRemote();
    } 
    
    /// <summary>
    /// Get elems that are not from new subs and have remote id = 0
    /// </summary>
    private List<ControlElementLocal> GetElemModelsToAdd()
    {
        List<ControlElementLocal> elems = [];
        
        foreach (Subject subj in _allSubjects)
        {
            if (SubjectsToAdd.ContainsKey(subj.LocalModel.SubjectId))
            {
                continue;
            }

            foreach (ControlElementLocal elemModel in subj.LocalModel.Elements)
            {
                if (elemModel.RemoteId == 0)
                {
                    elems.Add(elemModel);
                }
            }
        }

        return elems;
    }
}