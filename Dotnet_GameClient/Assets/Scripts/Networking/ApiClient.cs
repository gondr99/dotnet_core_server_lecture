using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Networking.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class ApiClient : MonoBehaviour
    {
        public static ApiClient Instance { get; private set; }
        
        [SerializeField] private string serverUrl = "http://localhost:5100";
        [SerializeField] private int timeoutSeconds = 10; //10초간 연결안되면 서버 꺼진거.
        
        public string ServerUrl => serverUrl;
        
        //서버쪽 메시지들이 CamelCase로 되어 있어서 그걸 기반으로 Resolving을 해야한다.
        //널 값의 프로퍼티는 무시해라(파싱하지 마라)
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        //로드되기전에 만약 ApiClient가 없다면 생성해준다.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if(Instance == null)
                new GameObject("ApiClient").AddComponent<ApiClient>();
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); //자기자신 삭제 금지.
        }


        #region Http 메서드 콜 - Post, GET

        public Task<ApiResult<TRes>> GetAsync<TRes>(string path, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbGET, path, null, ct);
        public Task<ApiResult<TRes>> PostAsync<TRes>(string path, object body, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbPOST, path, body, ct);
        public Task<ApiResult<TRes>> PutAsync<TRes>(string path, object body, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbPUT, path, body, ct);
        public Task<ApiResult<TRes>> DeleteAsync<TRes>(string path, CancellationToken ct = default)
            => SendAsync<TRes>(UnityWebRequest.kHttpVerbDELETE, path, null, ct);
        
        public async Task<ApiResult<TRes>> SendAsync<TRes>(string method, string path, object body,
            CancellationToken ct = default)
        {
            string url = $"{ServerUrl}/{path.TrimStart('/')}"; //접속할 풀 주소를 만들어준다.
            using UnityWebRequest req = new UnityWebRequest(url, method); //웹 요청 객체를 만들어준다.
            req.downloadHandler = new DownloadHandlerBuffer();
            req.timeout = timeoutSeconds; //대기 종료시간과 더불어 다운로드 버퍼를 할당해준다.

            if (body != null)  //POST요청의 경우 전송데이터가 있는데. 이걸 JSON형태로 보낸다.
            {
                string json = JsonConvert.SerializeObject(body, JsonSettings); //우리가 설정한 셋팅으로 직렬화
                req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)); //바이트 배열로 변환하여 업로딩
                
                req.SetRequestHeader("Content-type", "application/json"); // 이거 오타내면 절대 안된다.
            }
            
            req.SetRequestHeader("Accept", "application/json");

            try
            {
                await using (ct.Register(() => req.Abort()))
                    await req.SendWebRequest(); //요청을 실제 보내게 된다.
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ApiClient] {method}, {path} 예외 발생 : {e.Message}");
                throw;
            }
            
            //여기까지 온거면 성공적으로 데이터 전송이 일어난거니까
            long statusCode = req.responseCode;
            string text = req.downloadHandler?.text; //서버로부터 들어온 응답 텍스트를 받아주고

            //여기서 성공했다라는건 응답이 성공이란게 아니고, 데이터 통신이 성공했다라는 뜻이야.
            if (req.result == UnityWebRequest.Result.Success)
            {
                TRes data = default;
                if (!string.IsNullOrEmpty(text))
                    data = JsonConvert.DeserializeObject<TRes>(text, JsonSettings); //지정된 셋팅으로 TRes타입으로 파싱한다.

                return ApiResult<TRes>.Success(data, statusCode);
            }
            
            //여기까지 왔다라는건 응답이 실패했다. 
            ApiError error = ParseError(text, statusCode, req.error);
            Debug.LogWarning($"[ApiClient] {method}, {path} 실패 : {statusCode} / {error.Message}");
            
            return ApiResult<TRes>.Fail(error, statusCode);
        }

        private static ApiError ParseError(string body, long statusCode, string transportError)
        {
            //여기서는 서버가 준 데이터를 알맞게 파싱해서 ProblemDetail형태로 만들어서 전달해야 한다.
            ApiError error = new ApiError { Status = (int)statusCode, Raw = body };

            if (string.IsNullOrEmpty(body))
            {
                error.Message = string.IsNullOrEmpty(transportError) ? "요청 실패" : transportError;
                return error;
            }

            try
            {
                ProblemDetailsDto pd = JsonConvert.DeserializeObject<ProblemDetailsDto>(body, JsonSettings);

                if (pd != null)
                {
                    error.Message = !string.IsNullOrEmpty(pd.Message) ? pd.Message
                        : !string.IsNullOrEmpty(pd.Title) ? pd.Title
                        : "요청에 실패했습니다.";
                    error.Errors = pd.Errors;
                    return error;
                }
            }
            catch
            {
                //무시해도 된다.
            }

            error.Message = body;
            return error; //여기까지 왔다는건 올바르게 메시지 못받았다는 뜻.
        }

        #endregion
    }
}