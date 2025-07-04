namespace RateMeShared.Dto;

public class PlainElem
{
    public int RemoteId { get; set; }
    public required string Name { get; set; }
    public decimal Weight { get; set; }
    public decimal Grade { get; set; }
}