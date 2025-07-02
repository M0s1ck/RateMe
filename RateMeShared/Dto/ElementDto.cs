namespace RateMeShared.Dto;

public class ElementDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Weight { get; set; }
    public decimal Grade { get; set; }
}