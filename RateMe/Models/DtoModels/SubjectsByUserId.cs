namespace RateMe.Models.DtoModels;

public class SubjectsByUserId
{
    public int UserId { get; set; }
    public List<SubjectDto> Subjects { get; set; } = [];
}