using Jrd.GameStates.BuildingState.BuildingPanel;
using Unity.Entities;

namespace Jrd.UI.BuildingState
{
    [UpdateAfter(typeof(InitUISystem))]
    public partial struct BuildingPanelSystem : ISystem
    {
        private EntityManager _entityManager;
        private Entity _buildingPanelEntity;

        public void OnCreate(ref SystemState state)
        {
            _entityManager = state.EntityManager;
            _buildingPanelEntity = SystemAPI.GetSingletonEntity<BuildingPanelData>();

            state.RequireForUpdate<BuildingPanelData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var buildingPanelData = SystemAPI.GetComponentRO<BuildingPanelData>(_buildingPanelEntity).ValueRO;


            BuildingPanelMono.Instance.ShowElement(buildingPanelData.SetVisible);
        }
    }
}