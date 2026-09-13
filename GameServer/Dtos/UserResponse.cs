using GameServer.Entities;

namespace GameServer.Dtos;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public long Gold { get; set; }
    public int Level { get; set; } //캐릭터의 레벨
    public long Exp { get; set; } //캐릭터의 경험치
    public DateTime CreatedAt { get; set; }
    
    //엔티티 객체에서 응답 DTO로 전환해주는 정적 헬퍼 메서드
    public static UserResponse FromEntity(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Nickname = user.Nickname,
        Gold = user.Gold,
        Level = user.Character?.Level ?? 1,
        Exp = user.Character?.Exp ?? 0,
        CreatedAt = user.CreatedAt
    };
}