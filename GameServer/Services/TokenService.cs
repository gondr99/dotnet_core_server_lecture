using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameServer.Entities;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace GameServer.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    //토큰 생성해주는 서비스
    public (string token, DateTime expiresAt) CreateToken(User user)
    {
        string? key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
        string? issuer = _config["Jwt:Issuer"];
        string? audience = _config["Jwt:Audience"];
        int expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "120"); //기본 만료시간 120으로 할당
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes); //현재시간으로부터 120분 더한뒤를 만료시간으로 설정
        
        // 토큰 안에 담기는 주장(사실)들. 위조 불가능하게 적재할 것들(데이터 페이로드를 말함)
        Claim[]? claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //Subject의 약자로 토큰의 주체(소유자)를 의미
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username), //토큰 소유자의 이름
            new Claim("nickname", user.Nickname),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //토큰의 고유 아이디
        };

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );
        
        string tokenStr = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (tokenStr, expiresAt);
    }
}