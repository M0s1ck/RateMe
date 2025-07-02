namespace RateMe.Services.Interfaces;

public interface ISubjectUpdater
{
    Task SubjectsOverallRemoteUpdate();
    Task UpdateAllLocals();
    Task LoadAllUserSubjectsFromRemote();
    void RetainSubjectsToUpdate();
    void UpdateUserId(int newId);
}