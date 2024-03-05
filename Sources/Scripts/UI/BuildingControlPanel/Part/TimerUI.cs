using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Sources.Scripts.UI.BuildingControlPanel.Part
{
    public class TimerUI : IBcpTimer
    {
        private readonly VisualElement _timerProgress;
        private readonly Label _timerLabel;
        private readonly int _width = 130;
        private VisualElement timerContainer;

        public TimerUI(VisualElement panel)
        {
            timerContainer = panel.Q<VisualElement>("timer-cont");
            _timerProgress = timerContainer.Q<VisualElement>("pb-bar");
            _timerLabel = timerContainer.Q<Label>("text-label");
            _timerLabel.text = "";
            _timerProgress.style.width = 0;
        }

        public async void SetTimerText(float max, float value)
        {
            float previous = 0;
            var time = max;

            while (time >= 0)
            {
                var newWidth = 130 - ((130 / max) * time);

                _timerProgress.style.width = newWidth;

                _timerProgress.experimental.animation.Start(
                    new StyleValues { width = previous },
                    new StyleValues { width = newWidth },
                    500).Start();

                _timerLabel.text = time + "s";

                previous = newWidth;
                time--;
                await Task.Delay(1000);

                if (time == 0)
                {
                    _timerLabel.text = "Delivered!";
                    var a = timerContainer.experimental.animation.Scale(1.1f, 200);

                    a.onAnimationCompleted += () => timerContainer.experimental.animation.Scale(1f, 200).Start();
                    a.Start();
                }
            }


            await Task.Delay(1000);
            _timerLabel.text = "";
        }
    }

    public interface IBcpTimer
    {
        public void SetTimerText(float max, float value);
    }
}