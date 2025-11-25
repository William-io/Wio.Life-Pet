using Wio.Life_Pet.Transfer.Auth;
using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Abstraction.IApplication;

public interface IUserApplication
{
    public Task<Result<object>> GetAll(UserListRequest request);
    public Task<Result<int>> Create(UserCreateRequest request);
    public Task<Result<int>> Delete(UserDeleteRequest request);
    public Task<AuthResponse> Login(LoginRequest request);
}