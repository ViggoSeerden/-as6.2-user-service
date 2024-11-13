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
    public async Task<User> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id);
    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);
    public async Task UpdateAsync(User user) => _context.Users.Update(user);
    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        _context.Users.Remove(user);
    }
}