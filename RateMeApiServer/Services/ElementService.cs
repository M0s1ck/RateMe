using RateMeApiServer.Common;
using RateMeApiServer.Mappers;
using RateMeApiServer.Models.Entities;
using RateMeApiServer.Repositories;
using RateMeShared.Dto;

namespace RateMeApiServer.Services;

public class ElementService : IElementService
{
    private IElementsRepository _rep;

    public ElementService(IElementsRepository rep)
    {
        _rep = rep;
    }
    
    public async Task<DbInteractionResult<Dictionary<int, int>>> AddElemsAsync(Dictionary<int, IEnumerable<ElementDto>> elemDtosBySubjKeys)
    {
        Dictionary<int, Element[]> elemsBySubjKeys = ElementMapper.GetElemsBySubjKeys(elemDtosBySubjKeys);
        
        // Adding to db
        DbInteractionResult<Dictionary<int, Element[]>> interactionAdded = await _rep.AddElemsBySubjKeys(elemsBySubjKeys);

        if (interactionAdded.Status != DbInteractionStatus.Success)
        {
            return new DbInteractionResult<Dictionary<int, int>>(null, interactionAdded.Status);
        }
        
        // Saving updated remote keys
        Dictionary<int, Element[]> addedElemsBySubjKeys = interactionAdded.Value!;
        Dictionary<int, int> localRemoteKeys = GetLocalRemoteKeys(elemDtosBySubjKeys, addedElemsBySubjKeys);
        
        return new DbInteractionResult<Dictionary<int, int>>(localRemoteKeys, DbInteractionStatus.Success);
    }

    
    public async Task<DbInteractionStatus> UpdateElemsAsync(IEnumerable<PlainElem> elems)
    {
        return await _rep.UpdateElems(elems);
    }

    
    public async Task<DbInteractionStatus> RemoveElemsByKeys(IEnumerable<int> keys)
    {
        return await _rep.RemoveElemsByKeys(keys);
    }


    private Dictionary<int, int> GetLocalRemoteKeys(Dictionary<int, IEnumerable<ElementDto>> givenDtos, Dictionary<int, Element[]> added)
    {
        Dictionary<int, int> localRemoteKeys = [];
        
        foreach ((int subjKey, IEnumerable<ElementDto> dtos) in givenDtos)
        {
            if (!added.ContainsKey(subjKey))
            {
                continue;
            }

            IEnumerable<Element> elems = added[subjKey];

            foreach ((Element elem, ElementDto dto) in elems.Zip(dtos))
            {
                localRemoteKeys[dto.Id] = elem.Id;
            }
        }

        return localRemoteKeys;
    }
}