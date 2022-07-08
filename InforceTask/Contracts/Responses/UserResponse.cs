using InforceTask.Models;

namespace InforceTask.Contracts.Responses;

public sealed class UserResponse
{
	public Guid Id { get; }
	public string Username { get; }

	public UserResponse(User user)
	{
		this.Id = user.Id;
		this.Username = user.Username;
	}
}