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
    }
}