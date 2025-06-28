using RateMeApiServer.Common;
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
    
    public async Task<DbInteractionResult<SubjectsIds>> AddSubjectsAsync(int userId, IEnumerable<SubjectDto> subjects)
    {
        SubjectDto[] subjectsImport = subjects as SubjectDto[] ?? subjects.ToArray();
        
        List<Subject> subjectsToAdd = [];

        foreach (SubjectDto subjDto in subjectsImport)
        {
            Subject subj = SubjectMapper.GetSubjectFromDto(subjDto);
            subjectsToAdd.Add(subj);
        }
        
        // Adding to bd 
        DbInteractionResult<IEnumerable<Subject>> interaction = await _repository.AddSubjectsAsync(userId, subjectsToAdd);
        
        if (interaction.Status != DbInteractionStatus.Success)
        {
            return new DbInteractionResult<SubjectsIds>(null, interaction.Status);
        }
        
        Subject[] added = interaction.Value as Subject[] ?? interaction.Value!.ToArray();
        
        // Saving remote keys 
        SubjectsIds addedIds = new SubjectsIds();
        
        for (int i = 0; i < added.Length; ++i)
        {
            Subject addedSubj = added[i];
            SubjectDto importedSubj = subjectsImport[i];

            SubjectId subjId = SubjectMapper.GetSubjectId(importedSubj, addedSubj);
            addedIds.Subjects.Add(subjId);
        }

        return new DbInteractionResult<SubjectsIds>(addedIds, DbInteractionStatus.Success);
    }

    public async Task<DbInteractionStatus> RemoveSubjectsAsync(int userId, IEnumerable<int> keys)
    {
        return await _repository.RemoveSubjectsByKeys(userId, keys);
    }
}