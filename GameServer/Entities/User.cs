using System.ComponentModel.DataAnnotations;

namespace GameServer.Entities;

public class User
{
    //기본키가 될거고, 자동 증가하는게 될거야. 
    public int Id { get; set; }
    
    [Required]
    [MaxLength(40)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(40)]
    public string Nickname { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;
    
    //계정의 공용 재화. 
    public long Gold { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; //태평양 표준시로 현재시간을 기록
}
 