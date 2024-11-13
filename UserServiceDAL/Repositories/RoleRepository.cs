using Microsoft.EntityFrameworkCore;
using UserServiceBusiness.Interfaces;
using UserServiceDAL.DataContext;
using UserServiceBusiness.Models;

namespace UserServiceDAL.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;
    public RoleRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Role>> GetAllAsync() => await _context.Roles.ToListAsync();
    public async Task<Role> GetByIdAsync(Guid id) => await _context.Roles.FindAsync(id);
    public async Task AddAsync(Role role) => await _context.Roles.AddAsync(role);
    public async Task UpdateAsync(Role role) => _context.Roles.Update(role);
    public async Task DeleteAsync(Guid id)
    {
        var role = await GetByIdAsync(id);
        _context.Roles.Remove(role);
    }
}