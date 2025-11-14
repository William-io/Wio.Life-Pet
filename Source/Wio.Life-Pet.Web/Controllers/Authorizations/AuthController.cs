using Microsoft.AspNetCore.Mvc;
using Wio.Life_Pet.Abstraction.IApplication;
using Wio.Life_Pet.Transfer.Auth;

namespace Wio.Life_Pet.Web.Controllers.Authorizations;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserApplication _userApplication;
    
    public AuthController(IUserApplication userApplication)
    {
        _userApplication = userApplication;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var result = await _userApplication.Login(request);
        return Ok(result);
    }
}