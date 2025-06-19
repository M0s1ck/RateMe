using Microsoft.EntityFrameworkCore;
using RateMe.Api.Clients;
using RateMe.Api.Mappers;
using RateMe.Models.ClientModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;
using RateMeShared.Dto;

namespace RateMe.Api.Services;

public class SubjectsService
{
    private SubjectsClient _subjClient;
    private SubjectsRepository _rep = new();

    public SubjectsService(SubjectsClient subjClient)
    {
        _subjClient = subjClient;
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
    /// Pushes new subjects (from SubjectsToAdd) to remote bd
    /// </summary>
    public async Task PushSubjectsByUserId(int userId, Dictionary<int, Subject> subjectsToAdd)
    {
        SubjectsByUserId subjectsObj = new();
        subjectsObj.UserId = userId;

        foreach ((int key, Subject subj) in subjectsToAdd)
        {
            SubjectDto subjDto = SubjectMapper.GetSubjectDto(subj.LocalModel);    
            subjectsObj.Subjects.Add(subjDto);
        }
        
        List<SubjectId>? subjIds = await _subjClient.PushSubjectsByUserId(subjectsObj);
        
        // Updating remote keys if success
        if (subjIds != null)
        {
            await _rep.UpdateRemoteKeys(subjIds);
        }
    }
}