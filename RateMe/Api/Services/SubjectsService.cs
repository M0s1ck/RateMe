using Microsoft.EntityFrameworkCore;
using RateMe.Api.Clients;
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
    /// Pushes all local subjects to remote bd
    /// </summary>
    /// <param name="userId"></param>
    public async Task PushSubjectsByUserId(int userId)
    {
        // Building up Dto
        SubjectsByUserId subjectsObj = new();
        subjectsObj.UserId = userId;

        await using SubjectsContext context = new();
        
        foreach (SubjectLocal subj in context.Subjects.Include(s => s.Elements))
        {
            SubjectDto subjDto = new()
            {
                LocalId = subj.SubjectId,
                Name = subj.Name,
                Credits = subj.Credits,
            };

            foreach (ControlElementLocal elem in subj.Elements)
            {
                ControlElementDto elemDto = new ControlElementDto()
                {
                    LocalId = elem.ElementId,
                    Name = elem.Name,
                    Grade = elem.Grade,
                    Weight = elem.Weight
                };
                
                subjDto.Elements.Add(elemDto);
            }
            
            subjectsObj.Subjects.Add(subjDto);
        }
        
        // Pushing
        List<SubjectId>? subjIds = await _subjClient.PushSubjects(subjectsObj);
        
        // Updating remote keys if success
        if (subjIds != null)
        {
            await _rep.UpdateRemoteKeys(subjIds);
        }
    }
}