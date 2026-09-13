using System.ComponentModel.DataAnnotations;

namespace GameServer.Dtos;

public class GainExpRequest
{
    [Range(1, 1_000_000, ErrorMessage = "경험치는 1 이상이어야 합니다.")]
    public int Amount { get; set; }
}