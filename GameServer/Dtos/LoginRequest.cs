using System.ComponentModel.DataAnnotations;

namespace GameServer.Dtos;

public class LoginRequest
{
    [Required(ErrorMessage = "아이디는 필수입니다.")]
    public string Username { get; set; } = null!;
    
    [Required(ErrorMessage = "비밀번호는 필수입니다.")]
    public string Password { get; set; } = null!;
}