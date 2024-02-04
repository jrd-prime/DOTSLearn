using Jrd.GameStates.BuildingState;
using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.UI;
using Jrd.UI.BlueprintsShopPanel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Shop.BlueprintsShop
{
    [UpdateAfter(typeof(InitUISystem))]
    public partial struct BlueprintsShopPanelSystem : ISystem
    {
        private EntityManager _entityManager;
        private BuildingStateData _buildingStateData;
        private BlueprintsShopPanelData _blueprintsShopPanelData;
        private DynamicBuffer<BlueprintsBuffer> _blueprintsBuffers;
        private int _buildingsCount;

        public void OnCreate(ref SystemState state)
        {
            _entityManager = state.EntityManager;


            // state.RequireForUpdate<BuildingPanelData>();
            // state.RequireForUpdate<BuildPrefabsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // if (!SystemAPI.TryGetSingletonEntity<BuildingPanelData>(out var buildingPanelEntity)) return;

            _blueprintsShopPanelData = SystemAPI.GetSingletonRW<BlueprintsShopPanelData>().ValueRO;
            _buildingStateData = SystemAPI.GetSingletonRW<BuildingStateData>().ValueRO;
            _buildingsCount = _buildingStateData.BuildingPrefabsCount;

            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BlueprintsBuffer> buildingsPrefabsBuffers))
            {
                Debug.Log("NO BUFF " + this);
                return;
            }

            _blueprintsBuffers = buildingsPrefabsBuffers;

            {
                var instance = BlueprintsShopPanelUI.Instance;
                switch (instance.IsPanelVisible)
                {
                    case false when _blueprintsShopPanelData.SetVisible:
                        instance.InstantiateBuildingsCards(_buildingsCount, GetNamesList());
                        instance.SetElementVisible(true);
                        instance.SetPanelTitle("Panel Title");
                        break;
                    case true when !_blueprintsShopPanelData.SetVisible:
                        instance.SetElementVisible(false);
                        instance.ClearBuildingsCards();
                        break;
                }
            }
        }


        private NativeList<FixedString32Bytes> GetNamesList()
        {
            NativeList<FixedString32Bytes> namesList = new(_buildingsCount, Allocator.Temp);

            foreach (var building in _blueprintsBuffers)
            {
                namesList.Add(new FixedString32Bytes(building.Name));
            }

            return namesList;
        }
    }
}