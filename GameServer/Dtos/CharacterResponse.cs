using GameServer.Entities;

namespace GameServer.Dtos;

public class CharacterResponse
{
    public int Level { get; set; }
    public long Exp { get; set; }
    public long ExpToNextLevel { get; set; }

    public static CharacterResponse FromEntity(Character c, long expToNextLevel) 
        => new CharacterResponse
        {
            Level = c.Level,
            Exp = c.Exp,
            ExpToNextLevel = expToNextLevel
        };
}