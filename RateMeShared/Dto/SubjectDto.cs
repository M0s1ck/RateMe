namespace RateMeShared.Dto;

public class SubjectDto
{
    public int Id { get; set; }
    public int ForeignId { get; set; }
    public string Name { get; set; }
    public int Credits { get; set; }
    public List<ControlElementDto> Elements { get; set; } = [];
}