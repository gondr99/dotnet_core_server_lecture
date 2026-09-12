using System;

namespace Networking.Dtos
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
    }
    
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public long Gold { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}