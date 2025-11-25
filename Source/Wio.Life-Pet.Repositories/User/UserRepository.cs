using System.Data;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wio.Life_Pet.Abstraction.IRepository;
using Wio.Life_Pet.Transfer.Auth;
using Wio.Life_Pet.Transfer.Common;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Repository.User;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Connection")
                            ?? throw new ArgumentNullException(nameof(configuration),
                                "Connection string 'Connection' not found.");
        this._configuration = configuration;
    }

    public async Task<Result<object>> GetAll(UserListRequest request)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@p_index", request.Index);
            parameters.Add("@p_limit", request.Limit);
            
            var results = await connection.QueryAsync<dynamic>(
                "SP_LIST_USERS", parameters, commandType: CommandType.StoredProcedure);
            
            var resultList = results.ToList();
            
            if (!resultList.Any())
            {
                var emptyResponse = new 
                { 
                    isSuccess = true,
                    message = "Nenhum usuário encontrado.",
                    data = Enumerable.Empty<UserItemResponse>(),
                    totalRegisters = 0
                };
                return Result.Success(emptyResponse, "Consulta realizada com sucesso.");
            }
            
            int totalCount = (int)resultList.First().TotalRegisters;
            
            var userItems = resultList.Select(r => new UserItemResponse(
                Id: (int)r.Id,
                FirstName: (string)r.FirstName,
                LastName: (string)r.LastName,
                Email: (string)r.Email,
                Username: (string)r.Username,
                RoleId: (int)r.RoleId,
                State: (int)r.State
            ));

            var successResponse = new 
            { 
                isSuccess = true,
                message = $"Usuários carregados com sucesso. Total: {totalCount}",
                data = userItems,
                totalRegisters = totalCount
            };
            
            return Result.Success(successResponse);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>(
                new Error("User.GetAll.Error", $"Erro ao carregar usuários: {ex.Message}"));
        }
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

    public async Task<Result<UserDetailResponse>> GetUserByUsername(string username)
    {
        await using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@p_username", username);

        var user = await connection.QueryFirstOrDefaultAsync<UserDetailResponse>(
            "SP_GET_USER_BY_USERNAME", parameters, commandType: CommandType.StoredProcedure);

        if (user == null)
            return Result.Failure<UserDetailResponse>(new Error("User.GetByUsername.NotFound",
                "Usuário não encontrado."));

        return Result.Success(user);
    }

    public async Task<UserDetailResponse> ValidatingUser(LoginRequest request)
    {
        var user = await GetUserByUsername(request.Username);

        if (user.IsFailure || user.Value == null)
            throw new Exception("Usuário não encontrado.");

        return user.Value;
    }

    public Task<TokenResponse> Authenticate(UserDetailResponse request)
    {
        var key = _configuration.GetSection("JwtSettings:SecretKey").Value;
        
        if (key == null)
            throw new Exception("JWT Key não configurado!");

        var keyByte = Encoding.ASCII.GetBytes(key);

        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Name, request.Username));
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.RoleId.ToString()));
        
        var credentials = new SigningCredentials(new SymmetricSecurityKey(keyByte), SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = credentials
        };
        
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var tokenSetting = tokenHandler.CreateToken(tokenDescriptor);
        
        string token = tokenHandler.WriteToken(tokenSetting);

        return Task.FromResult(new TokenResponse { Token = token });
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await ValidatingUser(request);

        if (user == null)
            throw new Exception("Usuário ou senha inválidos.");

        var token = await Authenticate(user);

        return new AuthResponse { IsSuccess = true, User = user, Token = token.Token };
    }
}