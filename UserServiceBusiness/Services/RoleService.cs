using UserServiceBusiness.Interfaces;
using UserServiceBusiness.Models;

namespace UserServiceBusiness.Services;

public class RoleService(IRoleRepository roleRepository)
{
    public Task<IEnumerable<Role>> GetAllRolesAsync() => roleRepository.GetAllAsync();
    public Task<Role> GetRoleByIdAsync(Guid id) => roleRepository.GetByIdAsync(id);
    public Task AddRoleAsync(Role role) => roleRepository.AddAsync(role);
    public Task UpdateRoleAsync(Role role) => roleRepository.UpdateAsync(role);
    public Task DeleteRoleAsync(Guid id) => roleRepository.DeleteAsync(id);
}