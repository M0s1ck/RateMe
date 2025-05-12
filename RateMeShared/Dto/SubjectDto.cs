namespace RateMeShared.Dto;

public class SubjectDto
{
    public int LocalId { get; set; }
    public int RemoteId { get; set; }
    public string Name { get; set; }
    public int Credits { get; set; }
    public List<ControlElementDto> Elements { get; set; } = [];
}