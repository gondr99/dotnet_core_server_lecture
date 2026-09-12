using GameServer.Dtos;
using GameServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    // /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        UserResponse? result = await _authService.RegisterAsync(request);

        if (result is null)
        {
            return Conflict(new { message = "이미 사용중인 아이디 입니다." });
        }
        
        //REST API 규격은 반환시 Location 헤더에 새로 생성된 리소스의 URI를 포함해야합니다.
        // 첫번째가 Location, 뒤에가 반환 응답입니다. 
        return Created($"/api/users/{result.Id}", result);
    }
    //HTTP response code, 200, 300, 403 -> , 500
}