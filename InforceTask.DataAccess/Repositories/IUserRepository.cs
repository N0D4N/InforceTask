using InforceTask.Domain;

namespace InforceTask.DataAccess.Repositories;

public interface IUserRepository : IRepository
{
	Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
	Task AddUserAsync(User user, CancellationToken cancellationToken = default);
	Task<bool> IsUserAdminAsync(Guid id, CancellationToken cancellationToken = default);
}