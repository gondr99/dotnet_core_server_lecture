using System.ComponentModel.DataAnnotations;

namespace GameServer.Dtos;

//회원가입 요청
public class RegisterRequest
{
    [Required(ErrorMessage = "아이디는 필수입니다.")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "아이디는 3글자 이상 40자 이하입니다.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "닉네임은 필수입니다.")]
    [StringLength(40, MinimumLength = 2, ErrorMessage = "닉네임은 2글자 이상 40자 이하입니다.")]
    public string Nickname { get; set; } = null!;

    [Required(ErrorMessage = "비밀번호는 필수입니다.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "비밀번호는 6자 이상 100자 이하로 입력해야 합니다.")]
    public string Password { get; set; } = null!;
}