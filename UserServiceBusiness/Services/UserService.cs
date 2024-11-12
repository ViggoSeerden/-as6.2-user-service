using UserServiceBusiness.Interfaces;
using UserServiceBusiness.Models;

namespace UserServiceBusiness.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public Task<IEnumerable<User>> GetAllUsersAsync() => _userRepository.GetAllAsync();
    public Task<User> GetUserByIdAsync(string id) => _userRepository.GetByIdAsync(id);
    public Task AddUserAsync(User user) => _userRepository.AddAsync(user);
    public Task UpdateUserAsync(User user) => _userRepository.UpdateAsync(user);
    public Task DeleteUserAysnc(string id) => _userRepository.DeleteAsync(id);
}