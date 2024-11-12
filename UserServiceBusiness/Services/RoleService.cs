using UserServiceBusiness.Interfaces;
using UserServiceBusiness.Models;

namespace UserServiceBusiness.Services;

public class RoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    
    public Task<IEnumerable<Role>> GetAllRolesAsync() => _roleRepository.GetAllAsync();
    public Task<Role> GetRoleByIdAsync(string id) => _roleRepository.GetByIdAsync(id);
    public Task AddRoleAsync(Role role) => _roleRepository.AddAsync(role);
    public Task UpdateRoleAsync(Role role) => _roleRepository.UpdateAsync(role);
    public Task DeleteRoleAysnc(string id) => _roleRepository.DeleteAsync(id);
}