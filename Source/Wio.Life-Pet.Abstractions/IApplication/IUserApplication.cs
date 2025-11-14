using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Abstraction.IApplication;

public interface IUserApplication
{
    public Task<Result<UserListResponse>> GetAll();
    public Task<Result<int>> Create(UserCreateRequest request);
    public Task<Result<int>> Delete(UserDeleteRequest id);
}