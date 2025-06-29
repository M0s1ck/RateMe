namespace RateMeShared.Dto;

public class SubjectId
{
    public int LocalId { get; set; }
    public int RemoteId { get; set; }
    public List<ElementId> Elements { get; set; } = [];
}