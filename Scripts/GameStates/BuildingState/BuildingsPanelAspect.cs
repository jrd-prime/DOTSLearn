using Unity.Entities;

namespace Jrd.GameStates.BuildingState
{
    public readonly partial struct BuildingsPanelAspect : IAspect
    {
        public readonly Entity Self;

        private readonly RefRW<BSBuildingsPanelComponent> _buildingsPanelData;

        public bool DisplayVisibility
        {
            get { return _buildingsPanelData.ValueRO.Visibility; }
            set { _buildingsPanelData.ValueRW.Visibility = value; }
        }
    }
}