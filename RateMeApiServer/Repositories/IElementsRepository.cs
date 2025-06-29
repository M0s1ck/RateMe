using RateMeApiServer.Common;
using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Repositories;

public interface IElementsRepository
{
    /// <summary>
    /// Adds elements by their subjects' keys
    /// </summary>
    /// <returns>Added elements with updated keys</returns>
    public Task<DbInteractionResult<Dictionary<int, Element[]>>> AddElemsBySubjKeys(Dictionary<int, Element[]> elemsBySubjKeys);
    public Task<DbInteractionStatus> UpdateElems(IEnumerable<PlainElem> elems);
    public Task<DbInteractionStatus> RemoveElemsByKeys(IEnumerable<int> keys);
}