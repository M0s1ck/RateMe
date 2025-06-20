using RateMeApiServer.Mappers;
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
            Subject subj = SubjectMapper.GetSubjectFromDto(subjDto);
            subjectsToAdd.Add(subj);
        }
        
        // Adding to bd 
        List<Subject> added = await _repository.AddSubjectsAsync(subjectsImport.UserId, subjectsToAdd);
        
        // Saving remote keys 
        SubjectsIds addedIds = new SubjectsIds();
        
        for (int i = 0; i < added.Count; ++i)
        {
            Subject addedSubj = added[i];
            SubjectDto importedSubj = subjectsImport.Subjects[i];

            SubjectId subjId = SubjectMapper.GetSubjectId(importedSubj, addedSubj);
            addedIds.Subjects.Add(subjId);
        }

        return addedIds;
    }

    public async Task RemoveSubjectsAsync(PlainKeys keysObj)
    {
        await _repository.RemoveSubjectsByKeys(keysObj.Keys);
    }
}