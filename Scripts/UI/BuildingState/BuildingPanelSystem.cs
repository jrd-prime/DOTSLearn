using Jrd.GameStates.BuildingState.BuildingPanel;
using Unity.Collections;
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

            BuildingPanelMono.Instance.SetElementVisible(buildingPanelData.SetVisible);

            var names = new NativeList<FixedString32Bytes>(buildingPanelData.BuildingPrefabsCount,
                Allocator.Temp);
            names.Add("1x1");
            names.Add("2x2");
            names.Add("coll");
            names.Add("coll+rig");
            names.Add("coll+rig+kin");

            // UI_old.BuildingPanelUI.InstantiateButtons(buildingPanelData.BuildingPrefabsCount, names);

            names.Dispose();
        }
    }
}