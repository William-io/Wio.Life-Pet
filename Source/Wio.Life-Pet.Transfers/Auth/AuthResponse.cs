using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Transfer.Auth;

public record LoginResponse(UserDetailResponse User, string Token);