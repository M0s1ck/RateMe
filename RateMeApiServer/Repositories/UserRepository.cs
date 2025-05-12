using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Data;
using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                throw new InvalidDataException("User with such email already exists");
            }
            
            var entry = await _context.Users.AddAsync(user); // Mb just .Add ??
            await _context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        
        public async Task<User> AuthAsync(string email, string password)
        {
            User user = await _context.Users.FirstAsync(u => u.Email == email);
            
            if (user.Password != password)
            {
                throw new InvalidDataException("Wrong password");
            }

            return user;
        }
    }
}
