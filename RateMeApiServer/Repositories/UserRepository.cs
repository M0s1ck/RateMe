using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Common;
using RateMeApiServer.Data;
using RateMeApiServer.Models.Entities;
using RateMeShared.Dto;

namespace RateMeApiServer.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
        
        
    public async Task<DbInteractionResult<User>> GetByIdAsync(int id)
    {
        User? user = await _context.Users.FindAsync(id);

        if (user != null)
        {
            return new DbInteractionResult<User>(user, DbInteractionStatus.Success);
        }

        return new DbInteractionResult<User>(null, DbInteractionStatus.NotFound);
    }
        

    public async Task<DbInteractionResult<int>> AddAsync(User user)
    {
        bool alreadyExists = _context.Users.Any(u => u.Email == user.Email);
            
        if (alreadyExists)
        {
            return new DbInteractionResult<int>(default, DbInteractionStatus.Conflict);
        }
            
        var entry = _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new DbInteractionResult<int>(entry.Entity.Id, DbInteractionStatus.Success);
    }
        
        
    public async Task<DbInteractionResult<User>> AuthAsync(string email, string password)
    {
        try
        {
            User user = await _context.Users.FirstAsync(u => u.Email == email);
                
            if (user.Password != password)
            {
                return new DbInteractionResult<User>(null, DbInteractionStatus.WrongData);
            }
                
            return new DbInteractionResult<User>(user, DbInteractionStatus.Success);
        }
        catch (InvalidOperationException)
        {
            return new DbInteractionResult<User>(null, DbInteractionStatus.NotFound);
        }
    }

    public async Task<DbInteractionStatus> UpdateAsync(User user)
    {
        bool exists = _context.Users.Any(u => u.Id == user.Id);

        if (!exists)
        {
            return DbInteractionStatus.NotFound;
        }
        
        _context.Users.Attach(user);
        _context.Entry(user).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return DbInteractionStatus.Success;
    }

    public async Task<DbInteractionStatus> RemoveAsync(int id)
    {
        User? user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return DbInteractionStatus.NotFound;
        }

        _context.Users.Remove(user);
        
        await _context.SaveChangesAsync();
        return DbInteractionStatus.Success;
    }
}