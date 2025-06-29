using RateMeApiServer.Common;
using RateMeShared.Dto;

namespace RateMeApiServer.Services;

public interface IElementService
{
    public Task<DbInteractionResult<Dictionary<int, int>>> AddElemsAsync(Dictionary<int, IEnumerable<ElementDto>> elemDtosBySubjKeys);
    public Task<DbInteractionStatus> UpdateElemsAsync(IEnumerable<PlainElem> elems);
    public Task<DbInteractionStatus> RemoveElemsByKeys(IEnumerable<int> keys);
}