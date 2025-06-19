using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Api.Mappers;

internal static class SubjectMapper
{
    internal static SubjectDto GetSubjectDto(SubjectLocal subj)
    {
        SubjectDto subjDto = new()
        {
            LocalId = subj.SubjectId,
            Name = subj.Name,
            Credits = subj.Credits,
        };

        foreach (ControlElementLocal elem in subj.Elements)
        {
            ControlElementDto elemDto = ElementMapper.GetElementDto(elem);
            subjDto.Elements.Add(elemDto);
        }

        return subjDto;
    }
}