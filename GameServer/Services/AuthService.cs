using GameServer.Data;
using GameServer.Dtos;
using GameServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher; 
    //User타입에 대한 Haser를 등록한다.
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext db, IPasswordHasher<User> passwordHasher, TokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<UserResponse?> RegisterAsync(RegisterRequest request)
    {
        //이름 중복 검사.
        bool exists = await _db.Users.AnyAsync(user => user.Username == request.Username);
        if (exists)
            return null;
        
        //유저 엔티티 생성
        User user = new User
        {
            Username = request.Username,
            Nickname = request.Nickname,
            Character = new Character ()
            //골드와 Create at은 기본값을 사용한다.
        };
        
        //해시 비밀번호 생성(인자로 넘어가는 user는 구분용이고 사용하진 않아.)
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        
        
        //데이터베이스 저장
        _db.Users.Add(user);
        await _db.SaveChangesAsync(); //Insert sql 수행

        return UserResponse.FromEntity(user);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        //DB에 SELECT 쿼리가 만들어져서 날아간다.
        // SELECT * FROM users WHERE username = 'request.Username'
        User? user = await _db.Users
            .Include(u => u.Character)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
            return null;

        PasswordVerificationResult verifyResult =
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verifyResult == PasswordVerificationResult.Failed)
            return null;

        if (user.Character is null)
        {
            user.Character = new Character();
            await _db.SaveChangesAsync();
        }
        
        (string token, DateTime expiredAt) = _tokenService.CreateToken(user);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiredAt,
            User = UserResponse.FromEntity(user)
        };
    }
    
    public async Task<UserResponse?> GetProfileAsync(int userId)
    {
        User? user = await _db.Users
            .Include(u => u.Character)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user is null ? null : UserResponse.FromEntity(user);
    }
}