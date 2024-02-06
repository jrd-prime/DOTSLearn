using Jrd.Gameplay.Building.ControlPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage.Warehouse
{
    /// <summary>
    /// Set 
    /// </summary>
    public partial struct InitBuildingWarehouseProductsDataSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<JBuildingsPrefabsTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;

        public void OnUpdate(ref SystemState state)
        {
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
            // cache mb somewhere
            var requiredItems = SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
            var _manufacturedItems = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

            foreach (var (q, entity) in SystemAPI.Query<InitBuildingWarehouseProductsDataTag>().WithEntityAccess())
            {
                _bsEcb.RemoveComponent<InitBuildingWarehouseProductsDataTag>(entity);
                var a = new NativeParallelHashMap<int, int>(requiredItems.Length, Allocator.Persistent);

                foreach (var qq in requiredItems)
                {
                    a.Add((int)qq._item, 3);
                }

                _bsEcb.SetComponent(entity, new WarehouseProductsData
                {
                    Values = a
                });
            }
        }
    }
}