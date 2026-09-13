namespace GameServer.Dtos;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = null!;
}