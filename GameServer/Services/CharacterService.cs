using GameServer.Data;
using GameServer.Dtos;
using GameServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Services;

public class CharacterService
{
    private readonly AppDbContext _db;

    public CharacterService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Character?> GetOrCreateAsync(int userId)
    {
        Character? character = await _db.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

        if (character is not null)
            return character;

        bool userExists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return null; //유저가 없다면 만들어줄 필요 없다.
        
        character = new Character { UserId = userId }; //나머지는 기본값으로 처리
        _db.Characters.Add(character);
        await _db.SaveChangesAsync();
        return character;
    }

    public async Task<CharacterResponse?> AddExpAsync(int userId, int amount)
    {
        Character? character = await GetOrCreateAsync(userId);

        if (character is null)
            return null;

        character.Exp += amount;

        while (character.Exp >= Leveling.RequiredExp(character.Level))
        {
            character.Exp -= Leveling.RequiredExp(character.Level);
            character.Level++;
        }
        
        await _db.SaveChangesAsync();
        return CharacterResponse.FromEntity(character, Leveling.RequiredExp(character.Level));
    }
    
    public async Task<CharacterResponse?> GetByUserIdAsync(int userId)
    {
        Character? character = await GetOrCreateAsync(userId);
        return character is null 
            ? null 
            : CharacterResponse.FromEntity(character, Leveling.RequiredExp(character.Level));
    }
}