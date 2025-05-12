using RateMeApiServer.Data;
using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;
    
    public SubjectRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Subject>> AddSubjectsAsync(int userId, List<Subject> subjects)
    {
        User? user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            throw new InvalidDataException($"User with such id={userId} was not found");
        }

        foreach (Subject subj in subjects)
        {
            user.Subjects.Add(subj);
        }

        await _context.SaveChangesAsync();

        return subjects;
    }
}