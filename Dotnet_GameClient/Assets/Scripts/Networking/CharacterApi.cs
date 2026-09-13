using System.Threading;
using System.Threading.Tasks;
using Networking.Dtos;

namespace Networking
{
    public static class CharacterApi
    {
        //내 캐릭터 정보 조회
        public static Task<ApiResult<CharacterResponse>> MeAsync(CancellationToken ct = default)
            => ApiClient.Instance.GetAsync<CharacterResponse>("/api/character/me", ct);
        
        //경험치 획득 + 자동 레벨업 
        public static Task<ApiResult<CharacterResponse>> GainExpAsync(int amount, CancellationToken ct = default)
        {
            GainExpRequest reqBody = new GainExpRequest {Amount = amount};
            return ApiClient.Instance.PostAsync<CharacterResponse>(
                "/api/character/me/gain-exp", reqBody, ct);
        }
    }
}
