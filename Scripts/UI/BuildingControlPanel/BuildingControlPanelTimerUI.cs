using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public class BuildingControlPanelTimerUI : IBuildingTimer
    {
        private readonly ProgressBar _timerProgress;
        private readonly Label _timerLabel;

        public BuildingControlPanelTimerUI(VisualElement panel)
        {
            _timerProgress = panel.Q<ProgressBar>("timer-progress");
            _timerLabel = panel.Q<Label>("text-label");
            _timerLabel.text = "";
            _timerProgress.value = 100;
        }

        public void SetTimerText(float max, float value)
        {
            var round = Math.Round(value);

            Debug.LogWarning("set timer progress to " + round);
            _timerProgress.value = (float)((100 * round) / max);

            _timerLabel.text = round.ToString();


            if (round <= 0.3)
            {
                _timerLabel.text = "Delivered!";
            }
        }
    }

    public interface IBuildingTimer
    {
        public void SetTimerText(float max, float value);
    }
}