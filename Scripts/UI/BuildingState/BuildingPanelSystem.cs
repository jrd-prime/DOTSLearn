﻿using Jrd.GameStates.BuildingState;
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
        private BuildingStateData _buildingStateData;
        private BuildingPanelData _buildingPanelData;
        private DynamicBuffer<BuildingsBuffer> _buildingsPrefabsBuffers;
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

            _buildingPanelData = SystemAPI.GetSingletonRW<BuildingPanelData>().ValueRO;
            _buildingStateData = SystemAPI.GetSingletonRW<BuildingStateData>().ValueRO;
            _buildingsCount = _buildingStateData.BuildingPrefabsCount;

            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BuildingsBuffer> buildingsPrefabsBuffers))
            {
                Debug.Log("NO BUFF " + this);
                return;
            }

            _buildingsPrefabsBuffers = buildingsPrefabsBuffers;

            {
                var instance = BuildingPanelMono.Instance;
                switch (instance.IsPanelVisible)
                {
                    case false when _buildingPanelData.SetVisible:
                        instance.InstantiateBuildingsCards(_buildingsCount, GetNamesList());
                        instance.SetElementVisible(true);
                        instance.SetPanelTitle("Panel Title");
                        break;
                    case true when !_buildingPanelData.SetVisible:
                        instance.SetElementVisible(false);
                        instance.ClearBuildingsCards();
                        break;
                }
            }
        }


        private NativeList<FixedString32Bytes> GetNamesList()
        {
            NativeList<FixedString32Bytes> namesList = new(_buildingsCount, Allocator.Temp);

            foreach (var building in _buildingsPrefabsBuffers)
            {
                namesList.Add(new FixedString32Bytes(building.Name));
            }

            return namesList;
        }
    }
}