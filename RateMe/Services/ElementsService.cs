using RateMe.Api.Clients;
using RateMe.Api.Mappers;
using RateMe.Models.ClientModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;
using RateMe.Services.Interfaces;
using RateMeShared.Dto;

namespace RateMe.Services;

internal class ElementsService : ILocalElemService, IElemUpdater
{
    public ElementsClient ElemClient { get; set; }
    
    private readonly IEnumerable<Subject> _allSubjects;
    private List<ElementLocal> _elemsToUpdate = [];
    private HashSet<int> _elemKeysToRemove = [];

    private ElementsRepository _rep = new();
    private ElementsClient? _elemClient;
    
    internal ElementsService(IEnumerable<Subject> allSubjects)
    {
        _allSubjects = allSubjects;
    }

    public async Task ElementsOverallRemoteUpdate()
    {
        List<ElementLocal> elemsToAdd = GetElemsToAdd();
        
        if (elemsToAdd.Count != 0)
        {
            await PushElemsBySubjectsIds(elemsToAdd);
        }

        if (_elemsToUpdate.Count != 0)
        {
            await UpdateElemsRemote(_elemsToUpdate);
        }

        if (_elemKeysToRemove.Count != 0)
        {
            await RemoveElemsByKeysRemote(_elemKeysToRemove);
        }
    }
    
    
    private async Task PushElemsBySubjectsIds(List<ElementLocal> elems)
    {
        Dictionary<int, List<ElementDto>> dto = ElementMapper.GetElemsBySubIds(elems);
        
        // Pushing
        Dictionary<int, int>? localRemoteKeys = await ElemClient!.PushElemsBySubsIds(dto);

        // Updating remote keys
        if (localRemoteKeys != null)
        {
            await _rep.UpdateRemoteKeys(localRemoteKeys);
        }
    }
    
    private async Task UpdateElemsRemote(IEnumerable<ElementLocal> elemsToUpdate)
    {
        PlainElem[] elems = elemsToUpdate.Select(ElementMapper.GetPlainElem).ToArray();
        await ElemClient!.UpdateElems(elems);
    }
    
    private async Task RemoveElemsByKeysRemote(HashSet<int> subjectsKeys)                         
    {                                                                                             
        await ElemClient!.RemoveElemsByKeys(subjectsKeys);                                     
    }
    
    // Local stuff
    
    public async Task AddLocal(int subId, ElementLocal elem)
    {
        await _rep.Add(subId, elem);
    }

    public async Task RemoveLocal(ElementLocal elem)
    {
        if (elem.RemoteId != 0)
        {
            _elemKeysToRemove.Add(elem.RemoteId);
        }
        
        await _rep.Remove(elem);
    }

    public async Task AddLocals(int subId, IEnumerable<ElementLocal> elems)
    {
        await _rep.Add(subId, elems);
    }

    public async Task RemoveLocals(IEnumerable<ElementLocal> elems)
    {
        await _rep.Remove(elems);
    }


    public void RetainElemsToUpdate()
    {
        _elemsToUpdate = [];
        
        foreach (Subject subj in _allSubjects)
        {
            if (subj.LocalModel.RemoteId == 0)
            {
                continue;
            }

            foreach (ControlElement elem in subj.FormulaObj)
            {
                bool isDiff = elem.LocalModel.Name != elem.Name || elem.LocalModel.Weight != elem.Weight ||
                              elem.LocalModel.Grade != elem.Grade;
                
                if (elem.LocalModel.RemoteId != 0 && isDiff)
                {
                    _elemsToUpdate.Add(elem.LocalModel);
                }
            }
        }
    }
    

    /// <summary>
    /// Get elems that are not from new subs and have remote id = 0
    /// </summary>
    private List<ElementLocal> GetElemsToAdd()
    {
        List<ElementLocal> elems = [];
        
        foreach (Subject subj in _allSubjects)
        {
            if (subj.LocalModel.RemoteId == 0)
            {
                continue;
            }

            foreach (ElementLocal elemModel in subj.LocalModel.Elements)
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