using Jrd.GameStates.BuildingState;
using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
            // state.RequireForUpdate<BuildPrefabsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
          //  Debug.Log("in update !!!!! " + this);
            var buildingPanelData = SystemAPI.GetComponentRO<BuildingPanelData>(_buildingPanelEntity).ValueRO;
            var buildingStateData = SystemAPI.GetSingletonRW<BuildingStateData>().ValueRO;


            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BuildingsPrefabsBuffer > a))
            {
                Debug.Log("NO BUFF TOO " + this );
            }
            
            var names = new NativeList<FixedString32Bytes>(buildingPanelData.BuildingPrefabsCount, Allocator.Temp);
            foreach (var q in a)
            {
                names.Add(new FixedString32Bytes(q.PrefabName));
            }
          //  Debug.Log(buildingPanelData.SetVisible + " " + this);
            BuildingPanelMono.Instance.InstantiateButtons(buildingStateData.BuildingPrefabsCount, names);
            BuildingPanelMono.Instance.SetElementVisible(buildingPanelData.SetVisible);
            
            names.Dispose();
        }
    }
}