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
    private readonly IEnumerable<Subject> _allSubjects;
    private List<ControlElementLocal> _elemsToUpdate = [];
    private HashSet<int> _elemKeysToRemove = [];

    private ElementsRepository _rep = new();
    private ElementsClient? _elemClient;
    
    internal ElementsService(IEnumerable<Subject> allSubjects)
    {
        _allSubjects = allSubjects;
    }

    
    public async Task ElementsOverallRemoteUpdate()
    {
        List<ControlElementLocal> elemsToAdd = GetElemsToAdd();
        
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
    
    
    private async Task PushElemsBySubjectsIds(List<ControlElementLocal> elems)
    {
        Dictionary<int, List<ElementDto>> dto = ElementMapper.GetElemsBySubIds(elems);
        
        // Pushing
        Dictionary<int, int>? localRemoteKeys = await _elemClient!.PushElemsBySubsIds(dto);

        // Updating remote keys
        if (localRemoteKeys != null)
        {
            await _rep.UpdateRemoteKeys(localRemoteKeys);
        }
    }
    
    private async Task UpdateElemsRemote(IEnumerable<ControlElementLocal> elemsToUpdate)
    {
        PlainElem[] elems = elemsToUpdate.Select(ElementMapper.GetPlainElem).ToArray();
        await _elemClient!.UpdateElems(elems);
    }
    
    private async Task RemoveElemsByKeysRemote(HashSet<int> subjectsKeys)                         
    {                                                                                             
        await _elemClient!.RemoveElemsByKeys(subjectsKeys);                                     
    }

    public void UpdateUserId(int newId)
    {
        if (_elemClient == null)
        {
            _elemClient = new ElementsClient(newId);
            return;
        }
        
        _elemClient.UpdateUserId(newId);
    }
    
    
    public async Task AddLocal(int subId, ControlElementLocal elem)
    {
        await _rep.Add(subId, elem);
    }

    public async Task RemoveLocal(ControlElementLocal elem)
    {
        if (elem.RemoteId != 0)
        {
            _elemKeysToRemove.Add(elem.RemoteId);
        }
        
        await _rep.Remove(elem);
    }

    public async Task AddLocals(int subId, IEnumerable<ControlElementLocal> elems)
    {
        await _rep.Add(subId, elems);
    }

    public async Task RemoveLocals(IEnumerable<ControlElementLocal> elems)
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