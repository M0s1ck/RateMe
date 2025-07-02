using RateMe.Api.Clients;

namespace RateMe.Services.Interfaces;

public interface ISubjectUpdater
{
    SubjectsClient SubjClient { get; set; }
    Task SubjectsOverallRemoteUpdate();
    Task UpdateAllLocals();
    Task LoadUpdateAllUserSubjectsFromRemote();
    void RetainSubjectsToUpdate();
}