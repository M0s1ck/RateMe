using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Api.Mappers;

internal static class ElementMapper
{
    internal static ElementDto GetElementDto(ControlElementLocal elem)
    {
        return new ElementDto
        {
            LocalId = elem.ElementId,
            Name = elem.Name,
            Grade = elem.Grade,
            Weight = elem.Weight
        };
    }

    internal static Dictionary<int, List<ElementDto>> GetElemsBySubIds(List<ControlElementLocal> elems)
    {
        Dictionary<int, List<ElementDto>> dto = [];

        foreach (ControlElementLocal elem in elems)
        {
            if (!dto.ContainsKey(elem.Subject.RemoteId))
            {
                dto[elem.Subject.RemoteId] = [];
            }

            dto[elem.Subject.RemoteId].Add(GetElementDto(elem));
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