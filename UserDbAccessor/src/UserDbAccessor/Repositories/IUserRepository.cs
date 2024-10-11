

namespace UserDbAccessor.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(int userId);
        Task<User> UpdateUserPreferences(int userId, string preferences);
        Task<bool> DeleteUser(int userId);
    }
}
