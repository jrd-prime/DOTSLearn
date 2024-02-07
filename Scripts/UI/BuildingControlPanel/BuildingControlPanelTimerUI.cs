using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public class BuildingControlPanelTimerUI : IBuildingTimer
    {
        private readonly Label _timerLabel;

        public BuildingControlPanelTimerUI(VisualElement panel)
        {
            _timerLabel = panel.Q<Label>(BCPNamesID.SpecProductivityNameLabelId);
            _timerLabel.text = "construct";
        }

        public void SetTimerText(float value)
        {
            _timerLabel.text = $"{value} / hour";
        }
    }

    public interface IBuildingTimer
    {
        public void SetTimerText(float value);
    }
}