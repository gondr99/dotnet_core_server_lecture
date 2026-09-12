using System.Threading.Tasks;
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
            
            //나중에 여기에 자동로그인 추가됩니다.
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
                Debug.Log("로그인 처리는 아직입니다.");
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