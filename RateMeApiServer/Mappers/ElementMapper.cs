using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Mappers;

internal static class ElementMapper
{
    internal static ControlElement GetElementFromDto(ControlElementDto elemDto)
    {
        return new ControlElement
        {
            Name = elemDto.Name,
            Grade = elemDto.Grade,
            Weight = elemDto.Weight,
        };
    }

    internal static ControlElementId GetControlElementId(ControlElementDto importedLocal, ControlElement added)
    {
        return new ControlElementId
        {
            LocalId = importedLocal.LocalId,
            RemoteId = added.Id
        };
    }
}