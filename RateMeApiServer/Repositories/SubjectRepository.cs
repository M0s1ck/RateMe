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

    public async Task RemoveSubjectsByKeys(List<int> keys)
    {
        foreach (int key in keys)
        {
            Subject temp = new() { Id = key, Name = string.Empty };
            _context.Subjects.Attach(temp);
            _context.Subjects.Remove(temp);
        }
        
        await _context.SaveChangesAsync(); // Nothing will be removed, if one key doesn't exist in bd!  
    }
}