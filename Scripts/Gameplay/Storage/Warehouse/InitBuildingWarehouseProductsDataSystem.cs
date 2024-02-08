using Jrd.Gameplay.Building.ControlPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.Warehouse
{
    /// <summary>
    /// Set 
    /// </summary>
    public partial struct InitBuildingWarehouseProductsDataSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingsPrefabsBufferTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;

        public void OnUpdate(ref SystemState state)
        {
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var bufferEntity = SystemAPI.GetSingletonEntity<BuildingsPrefabsBufferTag>();
            // cache mb somewhere
            var requiredItems = SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
            var _manufacturedItems = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

            foreach (var (q, entity) in SystemAPI.Query<InitBuildingWarehouseProductsDataTag>().WithEntityAccess())
            {
                _bsEcb.RemoveComponent<InitBuildingWarehouseProductsDataTag>(entity);
                var requiredItemsMap = new NativeParallelHashMap<int, int>(requiredItems.Length, Allocator.Persistent);

                foreach (var qq in requiredItems)
                {
                    requiredItemsMap.Add((int)qq._item, 3);
                }

                _bsEcb.SetComponent(entity, new WarehouseProductsData { Values = requiredItemsMap });
            }
        }
    }
}