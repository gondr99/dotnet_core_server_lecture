namespace GameServer.Services;

public class Leveling
{
    public static long RequiredExp(int level) 
        => (long)Math.Pow(level, 2) * 100;
    
}