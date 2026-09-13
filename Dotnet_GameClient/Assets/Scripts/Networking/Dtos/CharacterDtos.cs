namespace Networking.Dtos
{
    public class CharacterResponse
    {
        public int Level { get; set; }
        public long Exp { get; set; }
        public long ExpToNextLevel { get; set; }
    }
    
    public class GainExpRequest
    {
        public int Amount { get; set; }
    }
}