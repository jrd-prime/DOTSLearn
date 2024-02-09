using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public class TimerUI : IBcpTimer
    {
        private readonly VisualElement _timerProgress;
        private readonly Label _timerLabel;
        private readonly int _width = 130;

        public TimerUI(VisualElement panel)
        {
            _timerProgress = panel.Q<VisualElement>("pb-bar");
            _timerLabel = panel.Q<Label>("text-label");
            _timerLabel.text = "";
            _timerProgress.style.width = 0;
        }

        public void SetTimerText(float max, float value)
        {
            var round = Math.Round(value);

            Debug.LogWarning("set timer progress to " + round);
            var t = _width / max;

            _timerProgress.style.width = 130 - (float)((_width * round) / max);

            _timerLabel.text = round.ToString() + "s";
            if (round <= 0.3)
            {
                _timerLabel.text = "Delivered!";
            }
        }
    }

    public interface IBcpTimer
    {
        public void SetTimerText(float max, float value);
    }
}