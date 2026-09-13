namespace GameServer.Entities;

public class Character
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!; //User와 1:1로 외래키로 묶인다.
    public int Level { get; set; } = 1;
    public long Exp { get; set; } = 0;
}