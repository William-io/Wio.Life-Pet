namespace Wio.Life_Pet.Transfer.User;

public record UserListResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string PasswordHash,
    int RoleId,
    int State
);