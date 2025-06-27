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

    internal static Dictionary<int, List<ControlElementDto>> GetElemsBySubIds(List<ControlElementLocal> elems)
    {
        Dictionary<int, List<ControlElementDto>> dto = [];

        foreach (ControlElementLocal elem in elems)
        {
            if (!dto.ContainsKey(elem.SubjectId))
            {
                dto[elem.SubjectId] = [];
            }

            dto[elem.SubjectId].Add(GetElementDto(elem));
        }

        return dto;
    }

    internal static PlainElem GetPlainElem(ControlElementLocal elem)
    {
        return new PlainElem()
        {
            RemoteId = elem.RemoteId,
            Name = elem.Name,
            Weight = elem.Weight,
            Grade = elem.Grade
        };
    }
}