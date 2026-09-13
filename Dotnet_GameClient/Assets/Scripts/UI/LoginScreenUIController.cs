using System.Threading.Tasks;
using CoreSystem;
using CoreSystem.Util;
using Networking;
using Networking.Dtos;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LoginScreenUIController : AbstractUIScreenController
    {
        private Button _tabLogin;
        private Button _tabRegister;
        private Button _submitBtn;

        private VisualElement _rowNickname;
        private VisualElement _rowPasswordConfirm;

        private TextField _username;
        private TextField _nickname;
        private TextField _password;
        private TextField _passwordConfirm;
        private Label _message;

        private bool _isRegisterMode;
        private bool _isBusy;
        
        protected override void Bind()
        {
            _tabLogin = Btn("tab-login");
            _tabRegister = Btn("tab-register");
            _submitBtn = Btn("submit-btn");

            _rowNickname = Q<VisualElement>("row-nickname");
            _rowPasswordConfirm = Q<VisualElement>("row-password-confirm");
            
            _username = Q<TextField>("field-username");
            _nickname = Q<TextField>("field-nickname");
            _password = Q<TextField>("field-password");
            _passwordConfirm = Q<TextField>("field-password-confirm");
            _message = Lbl("message-label");

            _tabLogin.clicked += () => SetRegisterMode(false);
            _tabRegister.clicked += () => SetRegisterMode(true);
            _submitBtn.clicked += () => HandleSubmit().Forget();
            
            SetRegisterMode(false);

            _ = TryAutoLogin();
        }

        private async Task TryAutoLogin()
        {
            if (!ApiClient.Instance.HasToken)
                return;
            
            SetBusy(true);
            SetMessage(_message, "자동 로그인 확인중 ...", isSuccess: true);

            ApiResult<UserResponse> result = await AuthApi.MeAsync();

            if (result.IsSuccess)
            {
                Session.CurrentUser = result.Data;
                SceneRouter.Go(SceneRouter.MainScene);
                return;
            }
            
            ApiClient.Instance.ClearToken(); //이 토큰은 문제가 있으니
            SetBusy(false);
            SetMessage(_message, "토큰이 올바르지 않습니다. 로그인해주세요", isSuccess:false);
        }

        private async Task HandleSubmit()
        {
            if (_isBusy) return;
            ClearMessage(_message);

            string username = _username.value?.Trim(); //입력한 값을 trim해서 넣어주고
            string password = _password.value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                SetMessage(_message, "아이디와 비밀번호를 입력하세요");
                return;
            }
            
            if(_isRegisterMode)
                await DoRegister(username, password);
            else
                await DoLogin(username, password);
        }

        private async Task DoLogin(string username, string password)
        {
            SetBusy(true);
            ApiResult<LoginResponse> result = await AuthApi.LoginAsync(username, password);
            SetBusy(false);

            if (result.IsSuccess)
            {
                Debug.Log($"[Login] 로그인 성공, 토큰 저장됨 = userId {result.Data.User?.Id}");
                Debug.Log($"[Token] {result.Data.Token}");
                
                //현재 유저를 세션에 저장하고 다른씬으로 넘겨야 한다.
                Session.CurrentUser = result.Data.User;
                SceneRouter.Go(SceneRouter.MainScene);
            }
            else
            {
                SetMessage(_message, result.Error.ToUserMessage());
            }
        }

        private async Task DoRegister(string username, string password)
        {
            string nickname = _nickname.value?.Trim();

            if (string.IsNullOrEmpty(nickname))
            {
                SetMessage(_message, "닉네임을 입력해야 합니다.");
                return;
            }

            if (password != _passwordConfirm.value)
            {
                SetMessage(_message, "비밀번호와 확인이 일치하지 않습니다.");
                return;
            }
            
            SetBusy(true); 
            ApiResult<UserResponse> result = await AuthApi.RegisterAsync(username, nickname, password);
            SetBusy(false);

            if (result.IsSuccess)
            {
                SetRegisterMode(false); //회원가입완료되었으니 로그인으로 전환
                _username.value = username;
                SetMessage(_message, "회원 가입 완료! 로그인해주세요", true);
            }
            else
            {
                SetMessage(_message, result.Error.ToUserMessage());
            }
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            _submitBtn.SetEnabled(!isBusy); //아예 버튼을 비활성화해서 보호한다.
        }

        private void SetRegisterMode(bool isRegisterMode)
        {
            _isRegisterMode = isRegisterMode;
            
            _tabLogin.EnableInClassList("tab-btn--active", !isRegisterMode);
            _tabRegister.EnableInClassList("tab-btn--active", isRegisterMode);
            
            _rowNickname.style.display = isRegisterMode ? DisplayStyle.Flex : DisplayStyle.None;
            _rowPasswordConfirm.style.display = isRegisterMode ? DisplayStyle.Flex : DisplayStyle.None;
            
            _submitBtn.text = isRegisterMode ? "회원가입" : "로그인";
            _submitBtn.EnableInClassList("btn--primary", !isRegisterMode);
            _submitBtn.EnableInClassList("submit-btn--register", isRegisterMode);
            
            ClearMessage(_message); //메시지 라벨에 있는 내용을 지워준다.
        }
    }
}