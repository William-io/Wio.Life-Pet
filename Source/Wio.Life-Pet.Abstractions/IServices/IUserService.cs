using Wio.Life_Pet.Transfer.Auth;
using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Abstraction.IServices;

public interface IUserService
{
    public Task<Result<object>> GetAll(UserListRequest request);
    public Task<Result<int>> Create(UserCreateRequest request);
    public Task<Result<int>> Delete(UserDeleteRequest id);
    public Task<AuthResponse> Login(LoginRequest request);
}