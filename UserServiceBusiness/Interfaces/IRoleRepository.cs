using UserServiceBusiness.Models;

namespace UserServiceBusiness.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> GetByIdAsync(string id);
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(string id);
}