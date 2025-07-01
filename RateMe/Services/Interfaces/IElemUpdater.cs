namespace RateMe.Services.Interfaces;

public interface IElemUpdater
{
    Task ElementsOverallRemoteUpdate();
    void RetainElemsToUpdate();
    void UpdateUserId(int newId);
}