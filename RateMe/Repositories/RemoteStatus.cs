namespace RateMe.Repositories;

public enum RemoteStatus
{
    UpToDate,
    ToAdd,  // Covered by RemoteId == 0
    ToUpdate,
    ToRemove // I'm lazy to actually implement that
}