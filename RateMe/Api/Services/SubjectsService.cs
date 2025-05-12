using Microsoft.EntityFrameworkCore;
using RateMe.Api.Clients;
using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Api.Services;

public class SubjectsService
{
    private SubjectsClient _subjClient;

    public SubjectsService(SubjectsClient subjClient)
    {
        _subjClient = subjClient;
    }
    
    public async Task PushSubjectsByUserId(int userId)
    {
        SubjectsByUserId subjectsObj = new();
        subjectsObj.UserId = userId;
        
        SubjectsContext context = new();
        
        foreach (SubjectLocal subj in context.Subjects.Include(s => s.Elements))
        {
            SubjectDto subjDto = new SubjectDto()
            {
                Name = subj.Name,
                Credits = subj.Credits,
            };

            foreach (ControlElementLocal elem in subj.Elements)
            {
                ControlElementDto elemDto = new ControlElementDto()
                {
                    Name = elem.Name,
                    Grade = elem.Grade,
                    Weight = elem.Weight
                };
                
                subjDto.Elements.Add(elemDto);
            }
            
            subjectsObj.Subjects.Add(subjDto);
        }

        await _subjClient.PushSubjects(subjectsObj);
    }
}