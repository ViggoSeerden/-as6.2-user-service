using UserServiceBusiness.Interfaces;
using UserServiceBusiness.Models;

namespace UserServiceBusiness.Services;

public class UserService(IUserRepository userRepository)
{
    public Task<IEnumerable<User>> GetAllUsersAsync() => userRepository.GetAllAsync();
    public Task<User> GetUserByIdAsync(Guid id) => userRepository.GetByIdAsync(id);
    public Task<User> GetUserByEmail(string email) => userRepository.GetByEmailAsync(email);
    public Task AddUserAsync(User user) => userRepository.AddAsync(user);
    public Task UpdateUserAsync(User user) => userRepository.UpdateAsync(user);
    public Task DeleteUserAsync(Guid id) => userRepository.DeleteAsync(id);
}