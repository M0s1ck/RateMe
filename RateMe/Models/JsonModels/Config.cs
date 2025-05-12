namespace RateMe.Models.JsonModels;

public class Config
{
    public bool IsSubjectsLoaded { get; set; }
    public bool IsFirstTime { get; set; }
    public required string ApiUrl { get; set; }
}