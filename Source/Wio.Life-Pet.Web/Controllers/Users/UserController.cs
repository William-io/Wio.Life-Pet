using Microsoft.AspNetCore.Mvc;
using Wio.Life_Pet.Abstraction.IApplication;
using Wio.Life_Pet.Transfer.User;

namespace Wio.Life_Pet.Web.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserApplication userApplication) : ControllerBase
{
    [HttpGet]
    [Route("List")]
    public async Task<IActionResult> ListUsersAsync()
    {
        var result = await userApplication.GetAll();
        return Ok(result.Value);
    }
    
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateRequest request)
    {
        var result = await userApplication.Create(request);
        return Ok(result.Value);
    }

    [HttpDelete]
    [Route("Delete")]
    public async Task<IActionResult> DeleteUserAsync([FromBody] UserDeleteRequest request)
    {
        var result = await userApplication.Delete(request);
        return Ok(result.Value);
    }

}