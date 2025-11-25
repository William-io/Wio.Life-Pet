using Wio.Life_Pet.Transfer.Auth;
using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Abstraction.IRepository;

public interface IUserRepository
{
   public Task<Result<object>> GetAll(UserListRequest request);
   public Task<Result<int>> Create(UserCreateRequest request);
   public Task<Result<int>> Delete(UserDeleteRequest id);
   
   public Task<Result<UserDetailResponse>> GetUserByUsername(string username);
   public Task<UserDetailResponse> ValidatingUser(LoginRequest request);
   public Task<TokenResponse> Authenticate(UserDetailResponse request);
   public Task<AuthResponse> Login(LoginRequest request);
}