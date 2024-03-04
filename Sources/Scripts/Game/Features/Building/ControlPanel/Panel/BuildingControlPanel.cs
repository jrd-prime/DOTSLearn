using Sources.Scripts.UI.BuildingControlPanel;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Panel
{
    public class BuildingControlPanel
    {
        public BuildingControlPanelUI UI;
        public BuildingButtons Buttons;
        public BuildingEvents Event;
        public BuildingUIUpdater UIUpdater;

        public BuildingControlPanel()
        {
            UI = BuildingControlPanelUI.Instance;
            Buttons = new BuildingButtons();
            UIUpdater = new BuildingUIUpdater();
            Event = new BuildingEvents(UIUpdater);
        }

        public void ProcessEvents(ref EventsDataWrapper eventsDataWrapper) =>
            Event.Process(ref eventsDataWrapper);
    }
}