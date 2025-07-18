using RateMe.Models.LocalDbModels;
using RateMeShared.Dto;

namespace RateMe.Api.MainApi.Mappers;

internal static class ElementMapper
{
    internal static ElementDto GetElementDto(ElementLocal elem)
    {
        return new ElementDto
        {
            Id = elem.ElementId,
            Name = elem.Name,
            Grade = elem.Grade,
            Weight = elem.Weight
        };
    }

    internal static ElementLocal GetLocalFromDto(ElementDto dto)
    {
        return new ElementLocal
        {
            RemoteId = dto.Id,
            Name = dto.Name,
            Grade = dto.Grade,
            Weight = dto.Weight
        };
    }

    internal static Dictionary<int, List<ElementDto>> GetElemsBySubIds(List<ElementLocal> elems)
    {
        Dictionary<int, List<ElementDto>> dto = [];

        foreach (ElementLocal elem in elems)
        {
            if (!dto.ContainsKey(elem.Subject.RemoteId))
            {
                dto[elem.Subject.RemoteId] = [];
            }

            dto[elem.Subject.RemoteId].Add(GetElementDto(elem));
        }

        return dto;
    }

    internal static PlainElem GetPlainElem(ElementLocal elem)
    {
        return new PlainElem
        {
            RemoteId = elem.RemoteId,
            Name = elem.Name,
            Weight = elem.Weight,
            Grade = elem.Grade
        };
    }
}