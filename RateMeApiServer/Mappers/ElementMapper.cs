using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Mappers;

internal static class ElementMapper
{
    internal static Element GetElementFromDto(ElementDto elemDto)
    {
        return new Element
        {
            Name = elemDto.Name,
            Grade = elemDto.Grade,
            Weight = elemDto.Weight,
        };
    }

    /// <summary>
    /// Gets dto with remote id of entity
    /// </summary>
    internal static ElementDto GetDto(Element elem)
    {
        return new ElementDto
        {
            Id = elem.Id,
            Name = elem.Name,
            Grade = elem.Grade,
            Weight = elem.Weight,
        };
    }

    internal static ElementId GetControlElementId(ElementDto importedLocal, Element added)
    {
        return new ElementId
        {
            LocalId = importedLocal.Id,
            RemoteId = added.Id
        };
    }

    internal static Dictionary<int, Element[]> GetElemsBySubjKeys(Dictionary<int, IEnumerable<ElementDto>> elemDtosBySubjKeys)
    {
        Dictionary<int, Element[]> elemsBySubjKeys = [];

        foreach ((int subjKey, IEnumerable<ElementDto> elemDtos) in elemDtosBySubjKeys)
        {
            IEnumerable<Element> elems = elemDtos.Select(GetElementFromDto);
            elemsBySubjKeys[subjKey] = elems.ToArray();
        }

        return elemsBySubjKeys;
    }
}