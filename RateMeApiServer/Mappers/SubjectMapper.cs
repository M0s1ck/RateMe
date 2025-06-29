using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Mappers;

internal static class SubjectMapper
{
    internal static Subject GetSubjectFromDto(SubjectDto subjDto)
    {
        Subject subj = new()
        {
            Name = subjDto.Name,
            Credits = subjDto.Credits,
        };

        foreach (ElementDto elemDto in subjDto.Elements)
        {
            Element elem = ElementMapper.GetElementFromDto(elemDto); 
            subj.Elements.Add(elem);
        }

        return subj;
    }

    internal static SubjectId GetSubjectId(SubjectDto importedLocal, Subject added)
    {
        SubjectId subjId = new()
        {
            LocalId = importedLocal.LocalId,
            RemoteId = added.Id
        };

        for (int j = 0; j < added.Elements.Count; ++j)
        {
            ElementId elemId = ElementMapper.GetControlElementId(importedLocal.Elements[j], added.Elements[j]);
            subjId.Elements.Add(elemId);
        }

        return subjId;
    }
}