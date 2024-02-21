using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.UI.BuildingControlPanel.Part
{
    public class TimerUI : IBcpTimer
    {
        private readonly VisualElement _timerProgress;
        private readonly Label _timerLabel;
        private readonly int _width = 130;

        public TimerUI(VisualElement panel)
        {
            var timerContainer = panel.Q<VisualElement>("timer-cont");
            _timerProgress = timerContainer.Q<VisualElement>("pb-bar");
            _timerLabel = timerContainer.Q<Label>("text-label");
            _timerLabel.text = "timer";
            _timerProgress.style.width = 33;
        }

        public async void SetTimerText(float max, float value)
        {
            var round = Math.Round(value);

            Debug.LogWarning("set timer progress to ");
            var t = _width / max;

            _timerProgress.style.width = 130 - (float)((_width * round) / max);

            _timerLabel.text = "In delivery: " + round + "s";
            if (round <= 0.3)
            {
                _timerLabel.text = "Delivered!";
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