using GamePlay.Building.Prefabs;
using GamePlay.GameStates;
using GamePlay.GameStates.BuildingState;
using GamePlay.UI.BlueprintsShopPanel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.Shop.BlueprintsShop
{
    [UpdateAfter(typeof(MyInitSystemGroup))]
    public partial struct BlueprintsShopPanelSystem : ISystem
    {
        private EntityManager _entityManager;
        private BuildingStateData _buildingStateData;
        private BlueprintsShopData _blueprintsShopData;
        private DynamicBuffer<BlueprintsBuffer> _blueprintsBuffers;
        private int _buildingsCount;

        public void OnCreate(ref SystemState state)
        {
            _entityManager = state.EntityManager;
            state.RequireForUpdate<BlueprintsBuffer>();

            // state.RequireForUpdate<BuildingPanelData>();
            // state.RequireForUpdate<BuildPrefabsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // if (!SystemAPI.TryGetSingletonEntity<BuildingPanelData>(out var buildingPanelEntity)) return;


            if (SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BlueprintsBuffer> buildingsPrefabsBuffers))
            {
                Debug.Log("HAS BUFF " + this);
            }
            else
            {
                Debug.Log("NO BUFF " + this);
                return;
            }


            if (SystemAPI.TryGetSingletonRW<BlueprintsShopData>(out var b))
            {
                Debug.Log(" NOUUU BlueprintsShopData");
            }
            else
            {
                Debug.Log(" YES BlueprintsShopData");
            }

            if (SystemAPI.TryGetSingletonRW<BuildingStateData>(out var a))
            {
                Debug.Log(" NOUUU BuildingStateData");
            }
            else
            {
                Debug.Log(" YES BuildingStateData");
            }

            _blueprintsShopData = b.ValueRO;
            _buildingStateData = a.ValueRO;

            _buildingsCount = _buildingStateData.BuildingPrefabsCount;

            _blueprintsBuffers = buildingsPrefabsBuffers;

            {
                var instance = BlueprintsShopPanelUI.Instance;
                switch (instance.IsPanelVisible)
                {
                    case false when _blueprintsShopData.SetVisible:
                        instance.InstantiateBuildingsCards(_buildingsCount, GetNamesList());
                        instance.SetElementVisible(true);
                        instance.SetPanelTitle("Panel Title");
                        break;
                    case true when !_blueprintsShopData.SetVisible:
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