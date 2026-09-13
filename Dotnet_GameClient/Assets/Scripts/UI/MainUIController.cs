using System.Threading.Tasks;
using CoreSystem;
using Networking;
using Networking.Dtos;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class MainUIController : AbstractUIScreenController
    {
        private VisualElement _settingsPopup;
        private Label _popupNickname;
        private Label _popupLevel;
        private bool _isPopupOpen;
        
        protected override void Bind()
        {
            _settingsPopup = Q<VisualElement>("settings-popup");
            _popupNickname = Lbl("popup-nickname");
            _popupLevel = Lbl("popup-level");

            Btn("menu-play").clicked += HandlePlayBtn;
            Btn("menu-option").clicked += HandleOptionBtn;
            Btn("menu-quit").clicked += HandleQuitBtn;
            Btn("gear-btn").clicked += HandleToggleSettings;
            Btn("logout-btn").clicked += HandleLogoutBtn;
            
            SetPopupOpen(false);
            _ = LoadUserInfo();
        }

        private async Task LoadUserInfo()
        {
            UserResponse user = Session.CurrentUser;

            if (user == null && ApiClient.Instance.HasToken)
            {
                ApiResult<UserResponse> result = await AuthApi.MeAsync(); //정보 새로 받아오고
                if (result.IsSuccess)
                {
                    user = result.Data;
                    Session.CurrentUser = user;
                }
            }

            if (user == null)
            {
                SceneRouter.Go(SceneRouter.LoginScene);
                return;
            }
            
            SetMessage(_popupNickname, $"닉네임 : {user.Nickname}");
            SetMessage(_popupLevel, $"골드 : {user.Gold}");
        }
        
        private void SetPopupOpen(bool isOpen)
        {
            _isPopupOpen = isOpen;
            _settingsPopup.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;
        }


        private void HandlePlayBtn() => SceneRouter.Go(SceneRouter.TownScene);

        private void HandleOptionBtn() => Debug.Log("옵션 버튼은 준비중입니다.");

        private void HandleQuitBtn()
        {
            Debug.Log("[MainUIController] QuitBtn Clicked");
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        private void HandleToggleSettings() => SetPopupOpen(!_isPopupOpen);

        private void HandleLogoutBtn()
        {
            ApiClient.Instance.ClearToken();
            Session.Clear();
            SceneRouter.Go(SceneRouter.LoginScene);
        }
    }
}