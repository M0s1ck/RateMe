using RateMe.Api.MainApi.Clients;

namespace RateMe.Services.Interfaces;

public interface ISubjectUpdater
{
    bool IsAnyData { get; }
    SubjectsClient SubjClient { set; }
    Task SubjectsOverallRemoteUpdate();
    Task LoadUpdateAllUserSubjectsFromRemote();
    Task UpdateAllLocals();
    Task ClearLocal();
    Task MarkRemoteStates();
    void RetainSubjectsToUpdate();
}