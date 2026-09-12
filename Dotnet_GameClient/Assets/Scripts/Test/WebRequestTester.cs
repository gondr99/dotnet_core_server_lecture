using System.Threading.Tasks;
using Networking;
using Networking.Dtos;
using UnityEngine;

namespace Test
{
    public class WebRequestTester : MonoBehaviour
    {
        [SerializeField] private string username;
        [SerializeField] private string nickname;
        [SerializeField] private string password;

        [ContextMenu("Test Web Request - Register" )]
        private async Task TestRegister()
        {
            Debug.Log("회원가입 테스트 중");
            ApiResult<UserResponse> result = await AuthApi.RegisterAsync(username, nickname, password);
            
            Debug.Log($"{result.Data.Id} 로 가입되었습니다. : {result.Data.Username}");
        }
    }
}