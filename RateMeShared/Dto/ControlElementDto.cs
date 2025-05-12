namespace RateMeShared.Dto;

public class ControlElementDto
{
    public int LocalId { get; set; }
    public string Name { get; set; }
    public decimal Weight { get; set; }
    public decimal Grade { get; set; }
    public int ForeignId { get; set; }
}