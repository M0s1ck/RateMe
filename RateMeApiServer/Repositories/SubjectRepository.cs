using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Common;
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
    
    
    public async Task<DbInteractionResult<IEnumerable<Subject>>> AddSubjectsAsync(int userId, IEnumerable<Subject> isubjects)
    {
        Subject[] subjects = isubjects as Subject[] ?? isubjects.ToArray();
        User? user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return new DbInteractionResult<IEnumerable<Subject>>(null, DbInteractionStatus.NotFound);
        }

        foreach (Subject subj in subjects)
        {
            user.Subjects.Add(subj);
        }

        await _context.SaveChangesAsync();
        return new DbInteractionResult<IEnumerable<Subject>>(subjects, DbInteractionStatus.Success);
    }

    
    public async Task<DbInteractionStatus> RemoveSubjectsByKeys(int userId, IEnumerable<int> ikeys)
    {
        HashSet<int> keys = ikeys as HashSet<int> ?? ikeys.ToHashSet();
        User? user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return DbInteractionStatus.NotFound;
        }

        await _context.Subjects.Where(s => keys.Contains(s.Id)).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
        return DbInteractionStatus.Success;
    }
}