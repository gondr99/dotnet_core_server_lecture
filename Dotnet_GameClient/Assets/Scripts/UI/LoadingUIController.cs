using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LoadingUIController : AbstractUIScreenController
    {
        [SerializeField] private float minDisplaySeconds = 1.2f; //스플래시 스크린이 너무 빨리 사라지지 않게 최소시간.

        private Label _statusLabel;
        private VisualElement _progressFill;
        
        private List<(string labelText, Func<Task> workFunc)> BuildSteps() => new ()
        {
            ("시스템 초기화...", () => Task.Delay(200)),
            ("리소스 준비중...", LoadAssets)
        };

        
        protected override void Bind()
        {
            _statusLabel = Lbl("status");
            _progressFill = Q<VisualElement>("progress-fill");
            SetProgress(0);
            _ = RunLoadingAsync();
        }

        private void SetProgress(float normalizedProgress)
        {
            if(_progressFill != null)
                _progressFill.style.width = Length.Percent(normalizedProgress * 100f);
        }

        private void SetStatus(string statusText)
        {
            if(_statusLabel != null)
                _statusLabel.text = statusText;
        }

        private async Task RunLoadingAsync()
        {
            float startTime = Time.realtimeSinceStartup; //시작하고 지난 시간
            List<(string labelText, Func<Task> workFunc)> steps = BuildSteps(); //함수에서 받아온다.

            for (int i = 0; i < steps.Count; i++)
            {
                SetStatus(steps[i].labelText);
                SetProgress((float)i / steps.Count);

                try
                {
                    await steps[i].workFunc(); //Func를 콜해준다.
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[Loading UI] Step : {steps[i].labelText} 진행중 경고 발생 : {e.Message}");
                }
                
                SetProgress((float)(i+1) / steps.Count);
            }
            
            float elapsed = Time.realtimeSinceStartup - startTime; //경과된 시간.
            if (elapsed < minDisplaySeconds)
                await Task.Delay(TimeSpan.FromSeconds(minDisplaySeconds - elapsed)); //남은 시간 대기
            
            SetStatus("Complete! 게임으로 들어갑니다.");
            
            //여기서 씬 전환 예정.
            SceneRouter.Go(SceneRouter.LoginScene);
        }

        private async Task LoadAssets()
        {
            //실제 어드레서블 등으로 에셋을 로드해야하나. 지금은 그냥 딜레이만 넣는다.
            await Task.Delay(400);
        }
    }
}