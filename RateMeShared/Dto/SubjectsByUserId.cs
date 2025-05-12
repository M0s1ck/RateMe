namespace RateMeShared.Dto;

public class SubjectsByUserId
{
    public int UserId { get; set; }
    public List<SubjectDto> Subjects { get; set; } = [];
}