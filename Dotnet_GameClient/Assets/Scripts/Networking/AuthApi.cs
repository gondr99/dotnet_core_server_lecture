using System.Threading;
using System.Threading.Tasks;
using Networking.Dtos;

namespace Networking
{
    public static class AuthApi
    {
        public static Task<ApiResult<UserResponse>> RegisterAsync(string username, string nickname, string password,
            CancellationToken ct = default)
        {
            RegisterRequest reqBody = new RegisterRequest
            {
                Username = username,
                Nickname = nickname,
                Password = password
            };
            
            return ApiClient.Instance.PostAsync<UserResponse>("/api/auth/register", reqBody, ct);
        }

        public static async Task<ApiResult<LoginResponse>> LoginAsync(string username, string password,
            CancellationToken ct = default)
        {
            LoginRequest reqBody = new LoginRequest
            {
                Username = username,
                Password = password
            };
            
            ApiResult<LoginResponse> result 
                = await ApiClient.Instance.PostAsync<LoginResponse>("/api/auth/login", reqBody, ct);
            
            //성공적으로 로그인되었다면 앞으로는 데이터 인증시에 값을 넣도록 토큰 셋팅
            if(result.IsSuccess && result.Data != null && !string.IsNullOrEmpty(result.Data.Token))
                ApiClient.Instance.SetToken(result.Data.Token); //토큰 셋팅 

            return result;
        }
        
        public static Task<ApiResult<UserResponse>> MeAsync(CancellationToken ct = default)
            => ApiClient.Instance.GetAsync<UserResponse>("/api/auth/me", ct);
    }
}