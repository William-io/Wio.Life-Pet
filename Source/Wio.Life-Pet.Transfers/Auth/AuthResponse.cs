using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Transfer.Auth;

public class AuthResponse
{
    public Boolean IsSuccess { get; set; }
    public UserDetailResponse User { get; set; } = null!;
    public string Token { get; set; } = null!;
}