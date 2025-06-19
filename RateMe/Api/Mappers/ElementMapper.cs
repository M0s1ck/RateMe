using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Api.Mappers;

internal static class ElementMapper
{
    internal static ControlElementDto GetElementDto(ControlElementLocal elem)
    {
        return new ControlElementDto()
        {
            LocalId = elem.ElementId,
            Name = elem.Name,
            Grade = elem.Grade,
            Weight = elem.Weight
        };
    }
}