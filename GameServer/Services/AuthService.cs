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

    public AuthService(AppDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
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
            //골드와 Create at은 기본값을 사용한다.
        };
        
        //해시 비밀번호 생성(인자로 넘어가는 user는 구분용이고 사용하진 않아.)
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        
        
        //데이터베이스 저장
        _db.Users.Add(user);
        await _db.SaveChangesAsync(); //Insert sql 수행

        return UserResponse.FromEntity(user);
    }
}