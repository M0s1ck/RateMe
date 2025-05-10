namespace RateMe.Models.DtoModels;

public class SubjectDto
{
    public string Name { get; set; }
    public int Credits { get; set; }
    public List<ControlElementDto> Elements { get; set; } = [];
}