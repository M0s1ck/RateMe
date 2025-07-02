using RateMe.Api.Clients;

namespace RateMe.Services.Interfaces;

public interface ISubjectUpdater
{
    SubjectsClient SubjClient { get; set; }
    Task SubjectsOverallRemoteUpdate();
    Task LoadUpdateAllUserSubjectsFromRemote();
    Task UpdateAllLocals();
    Task ClearLocal();
    void RetainSubjectsToUpdate();
}