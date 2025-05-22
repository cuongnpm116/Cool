using Cool.Application.Dtos.Users;

namespace Cool.Application.Interfaces.Services;

public interface IUserService
{
    Task<int> CreateUser(CreateUserModel model);
}
