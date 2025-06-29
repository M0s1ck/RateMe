using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Common;
using RateMeApiServer.Data;
using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Repositories;

public class ElementsRepository : IElementsRepository
{
    private readonly AppDbContext _context;
    
    public ElementsRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<DbInteractionResult<Dictionary<int, Element[]>>> AddElemsBySubjKeys(Dictionary<int, Element[]> elemsBySubjKeys)
    {
        int addedCnt = 0;
        
        foreach ((int subjKey, Element[] elems) in elemsBySubjKeys)
        {
            Subject? subject = await _context.Subjects.FindAsync(subjKey);

            if (subject == null)
            {
                elemsBySubjKeys.Remove(subjKey);
                continue;
            }

            foreach (Element elem in elems)
            {
                subject.Elements.Add(elem);
                ++addedCnt;
            }
        }

        await _context.SaveChangesAsync();

        if (addedCnt == 0)
        {
            return new DbInteractionResult<Dictionary<int, Element[]>>(null, DbInteractionStatus.NotFound);
        }

        return new DbInteractionResult<Dictionary<int, Element[]>>(elemsBySubjKeys, DbInteractionStatus.Success);
    }

    
    public async Task<DbInteractionStatus> UpdateElems(IEnumerable<PlainElem> elems)
    {
        Dictionary<int, PlainElem> updatedMap = elems.ToDictionary(elem => elem.RemoteId);
        HashSet<int> idSet = updatedMap.Keys.ToHashSet();

        IEnumerable<Element> entitiesToUpdate = _context.Elements.Where(el => idSet.Contains(el.Id));
        int cnt = 0;

        foreach (Element elem in entitiesToUpdate)
        {
            int id = elem.Id;
            PlainElem updatedData = updatedMap[id];
            elem.Grade = updatedData.Grade;
            elem.Name = updatedData.Name;
            elem.Weight = updatedData.Weight;
            ++cnt;
        }

        await _context.SaveChangesAsync();
        return cnt == 0 ? DbInteractionStatus.NotFound : DbInteractionStatus.Success;
    }

    
    public async Task<DbInteractionStatus> RemoveElemsByKeys(IEnumerable<int> ikeys)
    {
        HashSet<int> keys = ikeys as HashSet<int> ?? ikeys.ToHashSet();
        int removedCnt = await _context.Elements.Where(s => keys.Contains(s.Id)).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
        return removedCnt == 0 ? DbInteractionStatus.NotFound : DbInteractionStatus.Success;
    }
}