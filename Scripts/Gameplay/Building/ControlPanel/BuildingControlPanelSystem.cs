using System;
using Jrd.Gameplay.Storage;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Storage.Warehouse;
using Jrd.GameStates;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.PlayState;
using Jrd.Goods;
using Jrd.UI.BuildingControlPanel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.ControlPanel
{
    public partial struct BuildingControlPanelSystem : ISystem
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton sys;
        private EntityCommandBuffer ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<JBuildingsPrefabsTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            sys = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            ecb = sys.CreateCommandBuffer(state.WorldUnmanaged);

            var mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            var a = BuildingControlPanelUI.Instance;
            foreach (var (buildingData, warehouseData, entity) in SystemAPI
                         .Query<BuildingData, RefRO<WarehouseProductsData>>()
                         .WithAll<InitializeTag, SelectedBuildingTag>()
                         .WithEntityAccess())
            {
                var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
                // cache mb somewhere
                var requiredItemsBuffer = SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
                var manufacturedItemsBuffer = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

                a.SetLevel(buildingData.Level);
                a.SetProductivity(buildingData.ItemsPerHour);
                a.SetLoadCapacity(buildingData.LoadCapacity);
                a.SetStorageCapacity(buildingData.MaxStorage);

                // TODO think
                a.SetSpecName(Spec.Productivity, manufacturedItemsBuffer.ElementAt(0)._item.ToString());
                a.SetSpecName(Spec.LoadCapacity, requiredItemsBuffer.ElementAt(0)._item.ToString());
                a.SetSpecName(Spec.WarehouseCapacity, manufacturedItemsBuffer.ElementAt(0)._item.ToString());

                a.SetLineInfo(requiredItemsBuffer, manufacturedItemsBuffer);

                // Debug.LogWarning(m.ValueRO.Values.IsCreated);
                // Debug.LogWarning(m.ValueRO.Values.IsEmpty);

                // Set items to warehouse and storage in building control panel from main storage

                if (!mainStorageData.GetMatchingProducts(requiredItemsBuffer, out var resultList))
                {
                    throw new Exception("Empty or not created list for building control panel storage");
                }

                a.SetStorageItems(resultList);
                
                
                a.SetWarehouseItems(ax);
                resultList.Dispose();


                ecb.RemoveComponent<InitializeTag>(entity);
            }
        }
    }
}