using InforceTask.Domain;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.DataAccess.Repositories;

internal sealed class UserRepository : BaseRepository, IUserRepository
{
	public UserRepository(AppDbContext appDbContext) : base(appDbContext)
	{ }

	public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return this._appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		return this._appDbContext.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
	}

	public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
	{
		await this._appDbContext.Users.AddAsync(user, cancellationToken);
	}

	public Task<bool> IsUserAdminAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return this._appDbContext.Users.Where(x => x.Id == id).Select(x => x.IsAdmin).FirstOrDefaultAsync(cancellationToken);
	}
}