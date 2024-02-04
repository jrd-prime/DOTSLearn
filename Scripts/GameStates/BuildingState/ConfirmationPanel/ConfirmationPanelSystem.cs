using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.UI.PopUpPanels;
using Unity.Entities;

namespace Jrd.GameStates.BuildingState.ConfirmationPanel
{
    [UpdateBefore(typeof(BuildingStateSystem))]
    public partial struct ConfirmationPanelSystem : ISystem
    {
        private ConfirmationPanelData _confirmationPanelData;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<JBuildingsPrefabsTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _confirmationPanelData = SystemAPI.GetSingletonRW<ConfirmationPanelData>().ValueRO;

            var instance = ConfirmationPanelUI.Instance;

            switch (instance.IsPanelVisible)
            {
                case false when _confirmationPanelData.SetVisible:
                    instance.SetElementVisible(true);
                    break;
                case true when !_confirmationPanelData.SetVisible:
                    instance.SetElementVisible(false);
                    break;
            }
        }
    }
}