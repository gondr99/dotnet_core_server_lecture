using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameServer.Dtos;
using GameServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
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

    [HttpPost("login")] // Post /api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LoginResponse? result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new { message = "아이디 또는 비밀번호가 잘못되었습니다." });
        }
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")] //api/auth/me
    public async Task<IActionResult> Me()
    {
        string? userIdStr = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        _logger.LogInformation($"User ID from token: {userIdStr}");
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized(new { message = "로그인 정보가 존재하지 않습니다." });
        
        UserResponse? profile = await _authService.GetProfileAsync(userId);
        if (profile is null)
            return NotFound(new { message = "사용자를 찾을 수 없습니다." });  //404
        
        return Ok(profile);
    }
}