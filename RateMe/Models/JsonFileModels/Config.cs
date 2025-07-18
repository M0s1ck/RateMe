namespace RateMe.Models.JsonFileModels;

public class Config
{
    public bool IsSubjectsLoaded { get; set; }
    public bool IsFirstTime { get; set; }
    public required string ApiUrl { get; set; }
    public required string S3Url { get; set; }
}