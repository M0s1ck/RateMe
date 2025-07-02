using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Api.Mappers;

internal static class SubjectMapper
{
    internal static SubjectDto GetSubjectDto(SubjectLocal subj)
    {
        SubjectDto subjDto = new()
        {
            Id = subj.SubjectId,
            Name = subj.Name,
            Credits = subj.Credits,
        };

        foreach (ElementLocal elem in subj.Elements)
        {
            ElementDto elemDto = ElementMapper.GetElementDto(elem);
            subjDto.Elements.Add(elemDto);
        }

        return subjDto;
    }

    internal static SubjectLocal GetLocalFromDto(SubjectDto dto)
    {
        SubjectLocal subject = new()
        {
            RemoteId = dto.Id,
            Name = dto.Name,
            Credits = dto.Credits,
        };

        foreach (ElementDto elemDto in dto.Elements)
        {
            ElementLocal elem = ElementMapper.GetLocalFromDto(elemDto);
            subject.Elements.Add(elem);
        }

        return subject;
    }
}