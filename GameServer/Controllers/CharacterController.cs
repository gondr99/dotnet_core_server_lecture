using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameServer.Dtos;
using GameServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharacterController : ControllerBase
{
    private readonly CharacterService _characterService;

    public CharacterController(CharacterService characterService)
    {
        _characterService = characterService;
    }

    //헬퍼 
    private int? GetUserId()
    {
        string? sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.TryParse(sub, out int userId) ? userId : null;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (GetUserId() is not int userId)
            return Unauthorized();

        CharacterResponse? character = await _characterService.GetByUserIdAsync(userId);

        if (character is null)
            return NotFound(new { message = "캐릭터를 찾을 수 없습니다." });
        
        return Ok(character);
    }

    [HttpPost("me/gain-exp")]
    public async Task<IActionResult> GainExp([FromBody] GainExpRequest request)
    {
        if (GetUserId() is not int userId)
            return Unauthorized();
        
        CharacterResponse? character = await _characterService.AddExpAsync(userId, request.Amount);

        if (character is null)
            return NotFound(new { message = "캐릭터를 찾을 수 없습니다." });
        
        return Ok(character);
    }
}