namespace RateMeShared.Dto;

public class SubjectId
{
    public int LocalId { get; set; }
    public int RemoteId { get; set; }
    public List<ControlElementId> Elements { get; set; } = [];
}