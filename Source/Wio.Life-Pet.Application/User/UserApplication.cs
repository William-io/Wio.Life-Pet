using Wio.Life_Pet.Abstraction.IApplication;
using Wio.Life_Pet.Abstraction.IServices;
using Wio.Life_Pet.Transfer.Auth;
using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Application.User;

public class UserApplication : IUserApplication
{
    private readonly IUserService _userService;
    
    public UserApplication(IUserService userService) => _userService = userService;
    
    public async Task<Result<object>> GetAll(UserListRequest request)
    {
        return await _userService.GetAll(request);
    }

    public async Task<Result<int>> Create(UserCreateRequest request)
    {
        return await _userService.Create(request);
    }

    public async Task<Result<int>> Delete(UserDeleteRequest id)
    {
        return await _userService.Delete(id);
    }

    public Task<AuthResponse> Login(LoginRequest request)
    {
        return _userService.Login(request);
    }
}