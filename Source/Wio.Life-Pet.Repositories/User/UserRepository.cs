using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Wio.Life_Pet.Abstraction.IRepository;
using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Repository.User;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Connection")
                            ?? throw new ArgumentNullException(nameof(configuration),
                                "Connection string 'Connection' not found.");
    }

    public async Task<Result<UserListResponse>> GetAll()
    {
        await using var connection = new SqlConnection(_connectionString);
        
        var users = (await connection.QueryFirstAsync<UserListResponse>(
            "SP_LIST_USERS", commandType: CommandType.StoredProcedure));

        return users;
    }

    public async Task<Result<int>> Create(UserCreateRequest request)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_first_name", request.FirstName);
            parameters.Add("@p_last_name", request.LastName);
            parameters.Add("@p_email", request.Email);
            parameters.Add("@p_username", request.Username);
            parameters.Add("@p_password_hash", request.PasswordHash);
            parameters.Add("@p_role_id", request.RoleId);

            await using var lecture = await connection.ExecuteReaderAsync(
                "SP_CREATE_USER", parameters, commandType: CommandType.StoredProcedure);

            if (await lecture.ReadAsync())
            {
                var userId = Convert.ToInt32(lecture["Id"]);
                return Result.Create(userId);
            }
            
            return Result.Failure<int>(new Error("User.Create.NoData", "Nenhum dado retornado pela procedure."));
        }
        catch (Exception e)
        {
            return Result.Failure<int>(new Error("User.Create.Error", $"Erro interno: {e.Message}"));
        }
    }

    public async Task<Result<int>> Delete(UserDeleteRequest request)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_id", request.Id);

            await using var lecture = await connection.ExecuteReaderAsync(
                "SP_DELETE_USER", parameters, commandType: CommandType.StoredProcedure);

            if (await lecture.ReadAsync())
            {
                var rows = Convert.ToInt32(lecture["Id"]);
                return Result.Success(rows);
            }
            
            return Result.Failure<int>(new Error("User.Delete.NoData", "Nenhum dado retornado pela procedure."));
        }
        catch (Exception e)
        {
            return Result.Failure<int>(new Error("User.Delete.Error", $"Erro interno: {e.Message}"));
        }
    }
}