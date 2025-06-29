using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Common;
using RateMeApiServer.Data;
using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;
    
    public SubjectRepository(AppDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<DbInteractionResult<IEnumerable<Subject>>> AddSubjects(int userId, IEnumerable<Subject> isubjects)
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

    
    public async Task<DbInteractionStatus> UpdateSubjects(int userId, IEnumerable<PlainSubject> updated)
    {
        User? user = await _context.Users.Include(u => u.Subjects).FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return DbInteractionStatus.NotFound;
        }

        Dictionary<int, PlainSubject> updatedMap = [];

        foreach (PlainSubject subj in updated)
        {
            updatedMap[subj.RemoteId] = subj;
        }

        foreach (Subject subject in user.Subjects)
        {
            bool exists = updatedMap.TryGetValue(subject.Id, out PlainSubject? updatedData);

            if (!exists)
            {
                continue;
            }

            subject.Name = updatedData!.Name;
            subject.Credits = updatedData.Credits;
        }
        
        await _context.SaveChangesAsync();
        return DbInteractionStatus.Success;
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