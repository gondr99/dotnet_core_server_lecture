using System.Threading.Tasks;
using CoreSystem;
using Networking;
using Networking.Dtos;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class TownUIController : AbstractUIScreenController
    {
        private Label _level;
        private Label _nickname;
        private VisualElement _hpFill;
        private Label _hpText;
        private VisualElement _expFill;
        private Label _expText;

        private ScrollView _chatLog;
        private TextField _chatInput;
        private Button _tabNormal;
        private Button _tabGuild;
        private bool _isGuildChannel;
        
        protected override void Bind()
        {
            _level = Lbl("hud-level");
            _nickname = Lbl("hud-nickname");
            _hpFill = Q<VisualElement>("hp-fill");
            _hpText = Lbl("hp-text");
            _expFill = Q<VisualElement>("exp-fill");
            _expText = Lbl("exp-text");
            
            _chatLog = Q<ScrollView>("chat-log");
            _chatInput = Q<TextField>("chat-input");
            _tabNormal = Btn("chat-tab-normal");
            _tabGuild = Btn("chat-tab-guild");

            _tabNormal.clicked += () => SetGuildChannel(false);
            _tabGuild.clicked += () => SetGuildChannel(true);

            Btn("leave-btn").clicked += () => SceneRouter.Go(SceneRouter.MainScene);
            Btn("chat-send").clicked += OnSend;
            _chatInput.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    OnSend();
            });

            SetHp(100, 100);
            SetGuildChannel(false);
            AppendSystem("마을에 입장했습니다.");
            _ = LoadCharacter();
        }

        private void SetHp(int current, int max)
        {
            float percent = max <= 0 ? 0 : Mathf.Clamp01(current / (float)max) * 100f;
            _hpFill.style.width = Length.Percent(percent);
            _hpText.text = $"{current} / {max}";
        }
        
        private async Task LoadCharacter()
        {
            if (Session.CurrentUser != null)
                _nickname.text = Session.CurrentUser.Nickname;
            
            //세션에 넣어도 된다.
            ApiResult<CharacterResponse> meResult = await CharacterApi.MeAsync();
            
            if(meResult.IsSuccess && meResult.Data != null)
                ApplyCharacterToUI(meResult.Data);
            else
                AppendSystem("캐릭터 정보를 불러오지 못했습니다.");
        }

        private void ApplyCharacterToUI(CharacterResponse characterData)
        {
            _level.text = $"LV. {characterData.Level}";
            long requiredExp = characterData.ExpToNextLevel <= 0 ? 1 : characterData.ExpToNextLevel;
            float percent = Mathf.Clamp01(characterData.Exp / (float)requiredExp) * 100f;
            
            _expFill.style.width = Length.Percent(percent);
            _expText.text = $"{characterData.Exp} / {requiredExp}";
        }

        #region Chat api

        private void AppendChat(string line) => AppendLine(line, false);
        private void AppendSystem(string message)=> AppendLine(message, true);

        private void AppendLine(string line, bool isSystem)
        {
            Label chatLabel = new Label(line);
            chatLabel.AddToClassList("chat-line");
            if(isSystem)
                chatLabel.AddToClassList("chat-line--system");
            _chatLog.Add(chatLabel);
            //나중에 자동 스크롤 다운까지 만들어볼께. 
            //UI 툴킷 스케쥴링
        }
        
        //데이터 전송
        private void OnSend()
        {
            
        }

        private void SetGuildChannel(bool isGuildChannel)
        {
            _isGuildChannel = isGuildChannel;
            _tabNormal.EnableInClassList("chat-tab--active", !isGuildChannel);
            _tabGuild.EnableInClassList("chat-tab--active", isGuildChannel);
        }
        #endregion
    }
}