namespace RateMeShared.Dto;

public class ElementDto
{
    public int LocalId { get; set; }
    public required string Name { get; set; }
    public decimal Weight { get; set; }
    public decimal Grade { get; set; }
}