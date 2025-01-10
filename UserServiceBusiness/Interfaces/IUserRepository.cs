using UserServiceBusiness.Models;

namespace UserServiceBusiness.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task Update(User user);
    Task DeleteAsync(Guid id);
}