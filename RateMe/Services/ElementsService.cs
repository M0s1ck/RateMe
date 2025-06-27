using RateMe.Api.Clients;
using RateMe.Api.Mappers;
using RateMe.Models.ClientModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;
using RateMeShared.Dto;

namespace RateMe.Services;

internal class ElementsService
{
    private readonly IEnumerable<Subject> _allSubjects;
    private List<ControlElementLocal> _elemsToUpdate = [];
    private List<int> _elemKeysToRemove = [];

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
        Dictionary<int, List<ControlElementDto>> dto = ElementMapper.GetElemsBySubIds(elems);
        await _elemClient.PushElemsBySubsIds(dto);
    }

    
    private async Task UpdateElemsRemote(IEnumerable<ControlElementLocal> elemsToUpdate)
    {
        PlainElem[] elems = elemsToUpdate.Select(ElementMapper.GetPlainElem).ToArray();
        await _elemClient.UpdateElems(elems);
    }
    
    private async Task RemoveElemsByKeysRemote(List<int> subjectsKeys)                         
    {                                                                                             
        await _elemClient.RemoveElemsByKeys(subjectsKeys);                                     
    }                                                                                             


    internal async Task AddLocal(int subId, ControlElementLocal elem)
    {
        await _rep.Add(subId, elem);
    }

    internal async Task RemoveLocal(ControlElementLocal elem)
    {
        if (elem.RemoteId != 0)
        {
            _elemKeysToRemove.Add(elem.RemoteId);
        }
        
        await _rep.Remove(elem);
    }
    
    
    internal void RetainElemsToUpdate()
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