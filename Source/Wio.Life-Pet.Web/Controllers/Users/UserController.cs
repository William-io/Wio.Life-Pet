using Microsoft.AspNetCore.Mvc;
using Wio.Life_Pet.Abstraction.IApplication;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Web.Controllers.Users;

[ApiController]
[Route("api/user")]
public class UserController(IUserApplication userApplication) : ControllerBase
{
    [HttpPost]
    [Route("list")]
    public async Task<IActionResult> ListUsersAsync([FromBody] UserListRequest request)
    {
        var result = await userApplication.GetAll(request);
        return Ok(result.Value);
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateRequest request)
    {
        var result = await userApplication.Create(request);
        return Ok(result.Value);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteUserAsync([FromBody] UserDeleteRequest request)
    {
        var result = await userApplication.Delete(request);
        return Ok(result.Value);
    }

}