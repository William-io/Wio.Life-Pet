namespace Wio.Life_Pet.Transfer.User;

public record UserListPaginatedResponse(
    IEnumerable<UserItemResponse> Data,
    int TotalRegisters
);

public record UserItemResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Username,
    int RoleId,
    int State
);