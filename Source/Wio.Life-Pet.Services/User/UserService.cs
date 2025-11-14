using Wio.Life_Pet.Abstraction.IRepository;
using Wio.Life_Pet.Abstraction.IServices;
using Wio.Life_Pet.Transfer.Auth;
using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository) => _userRepository = userRepository;
    
    public async Task<Result<UserListResponse>> GetAll()
    {
        return await _userRepository.GetAll();
    }

    public async Task<Result<int>> Create(UserCreateRequest request)
    {
        return await _userRepository.Create(request);
    }

    public async Task<Result<int>> Delete(UserDeleteRequest id)
    {
        return await _userRepository.Delete(id);
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        return await _userRepository.Login(request);
    }
}