using RateMe.Api.Clients;

namespace RateMe.Services.Interfaces;

public interface IElemUpdater
{
    ElementsClient ElemClient { get; set; }
    Task ElementsOverallRemoteUpdate();
    Task MarkRemoteStates();
    void RetainElemsToUpdate();
}