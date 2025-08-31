using RateMe.Api.MainApi.Clients;

namespace RateMe.Services.Interfaces;

public interface IElemUpdater
{
    ElementsClient ElemClient { set; }
    Task ElementsOverallRemoteUpdate();
    Task MarkRemoteStates();
    void RetainElemsToUpdate();
}