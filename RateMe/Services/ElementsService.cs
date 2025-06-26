using RateMe.Api.Clients;
using RateMe.Api.Mappers;
using RateMe.Models.ClientModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;
using RateMeShared.Dto;

namespace RateMe.Services;

internal class ElementsService
{
    internal List<ControlElementLocal> ElemsToRemove { get; } = [];
    
    private IEnumerable<Subject> _allSubjects;

    private ElementsRepository _rep = new();
    private ElementsClient _elemClient = new();
    
    internal ElementsService(IEnumerable<Subject> allSubjects)
    {
        _allSubjects = allSubjects;
    }

    
    internal async Task ElementsOverallRemoteUpdate()
    {
        List<ControlElementLocal> elemsToAdd = GetElemsToAdd();
        
        if (elemsToAdd.Count != 0)
        {
            await PushElemsBySubjectsIds(elemsToAdd);
        }
    }
    
    
    private async Task PushElemsBySubjectsIds(List<ControlElementLocal> elems)
    {
        Dictionary<int, List<ControlElementDto>> dto = ElementMapper.GetElemsBySubIds(elems);
        await _elemClient.PushElemsBySubsIds(dto);
    }


    internal async Task AddLocal(int subId, ControlElementLocal elem)
    {
        await _rep.Add(subId, elem);
    }
    
    
    /// <summary>
    /// Get elems that are not from new subs and have remote id = 0
    /// </summary>
    private List<ControlElementLocal> GetElemsToAdd()
    {
        List<ControlElementLocal> elems = [];
        
        foreach (Subject subj in _allSubjects)
        {
            if (subj.LocalModel.RemoteId == 0)
            {
                continue;
            }

            foreach (ControlElementLocal elemModel in subj.LocalModel.Elements)
            {
                if (elemModel.RemoteId == 0)
                {
                    elems.Add(elemModel);
                }
            }
        }

        return elems;
    }
}