using Microsoft.EntityFrameworkCore;
using UserServiceBusiness.Interfaces;
using UserServiceDAL.DataContext;
using UserServiceBusiness.Models;

namespace UserServiceDAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;
    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();
    public async Task<User?> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id) ?? null;
    public async Task<User?> GetByEmailAsync(string email) => await _context.Users
        .FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task Update(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
            throw new Exception();
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}