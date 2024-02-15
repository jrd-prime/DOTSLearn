using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public class ProductionTimersUI
    {
        private readonly VisualElement _timerProgress;
        private readonly Label _timerLabel;
        private readonly int _width = 130;

        // all
        private VisualElement _allTimerProgress;
        private Label _allTimerLabel;

        // one
        private VisualElement _oneTimerProgress;
        private Label _oneTimerLabel;

        public ProductionTimersUI(VisualElement panel)
        {
            var allTimerContainer = panel.Q<VisualElement>("all-timer-cont");
            _allTimerProgress = allTimerContainer.Q<VisualElement>("pb-bar");
            _allTimerLabel = allTimerContainer.Q<Label>("text-label");

            var oneTimerContainer = panel.Q<VisualElement>("one-timer-cont");
            _oneTimerProgress = oneTimerContainer.Q<VisualElement>("pb-bar");
            _oneTimerLabel = oneTimerContainer.Q<Label>("text-label");

            _allTimerProgress.style.width = 0;
            _oneTimerProgress.style.width = 0;
            _allTimerLabel.text = "init";
            _oneTimerLabel.text = "init";
        }

        public void UpdateTimers(float all, float one)
        {
            _allTimerLabel.text = all + " sec.";
            _oneTimerLabel.text = one + " sec.";
        }
    }
}