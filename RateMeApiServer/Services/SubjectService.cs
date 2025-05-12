using RateMeApiServer.Models.Entities;
using RateMeApiServer.Repositories;
using RateMeShared.Dto;

namespace RateMeApiServer.Services;

public class SubjectService : ISubjectService
{
    private ISubjectRepository _repository;

    public SubjectService(ISubjectRepository rep)
    {
        _repository = rep;
    }
    
    
    public async Task<SubjectsIds> AddSubjectsAsync(SubjectsByUserId subjectsImport)
    {
        List<Subject> subjectsToAdd = [];

        foreach (SubjectDto subjDto in subjectsImport.Subjects)
        {
            Subject subj = new()
            {
                Name = subjDto.Name,
                Credits = subjDto.Credits,
            };

            foreach (ControlElementDto elemDto in subjDto.Elements)
            {
                ControlElement elem = new()
                {
                    Name = elemDto.Name,
                    Grade = elemDto.Grade,
                    Weight = elemDto.Weight,
                };
                
                subj.Elements.Add(elem);
            }
            
            subjectsToAdd.Add(subj);
        }

        List<Subject> added = await _repository.AddSubjectsAsync(subjectsImport.UserId, subjectsToAdd);

        SubjectsIds addedIds = new SubjectsIds();
        
        for (int i = 0; i < added.Count; ++i)
        {
            Subject addedSubj = added[i];
            SubjectDto importedSubj = subjectsImport.Subjects[i];
            
            SubjectId subjId = new()
            {
                LocalId = importedSubj.LocalId,
                RemoteId = addedSubj.Id
            };

            for (int j = 0; j < addedSubj.Elements.Count; ++j)
            {
                ControlElementId elemId = new()
                {
                    LocalId = importedSubj.Elements[j].LocalId,
                    RemoteId = addedSubj.Elements[j].Id
                };
                
                subjId.Elements.Add(elemId);
            }
            
            addedIds.Subjects.Add(subjId);
        }

        return addedIds;
    }
}