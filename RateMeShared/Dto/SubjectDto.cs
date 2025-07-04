namespace RateMeShared.Dto;

public class SubjectDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Credits { get; set; }
    public List<ElementDto> Elements { get; set; } = [];
}