namespace RateMe.Services.Interfaces;

public interface ISubjectUpdater
{
    Task SubjectsOverallRemoteUpdate();
    Task UpdateAllLocals();
    void RetainSubjectsToUpdate();
    void UpdateUserId(int newId);
}