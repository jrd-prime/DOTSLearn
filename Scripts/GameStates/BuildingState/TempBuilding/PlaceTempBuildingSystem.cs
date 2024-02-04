using Jrd.Gameplay.Building;
using Jrd.Gameplay.Storage.Warehouse;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    /// <summary>
    /// Place temp building prefab
    /// </summary>
    [BurstCompile]
    public partial struct PlaceTempBuildingSystem : ISystem
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlaceTempBuildingTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameBuildingsData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsMap = SystemAPI
                .GetSingletonRW<GameBuildingsData>().ValueRW.GameBuildings;

            foreach (var (buildingData, transform, entity) in SystemAPI
                         .Query<RefRW<BuildingData>, RefRO<LocalTransform>>()
                         .WithAll<PlaceTempBuildingTag, TempBuildingTag>()
                         .WithEntityAccess())
            {
                float3 position = transform.ValueRO.Position;
                FixedString64Bytes guid = buildingData.ValueRO.Guid;
                BuildingData building = buildingData.ValueRO;

                buildingData.ValueRW.WorldPosition = position;

                _bsEcb.SetName(entity, $"{building.NameId}_{guid}");
                _bsEcb.AddComponent<BuildingTag>(entity);

                _bsEcb.AddComponent<WarehouseProductsData>(entity);
                _bsEcb.AddComponent<InitBuildingWarehouseProductsDataTag>(entity);

                _bsEcb.AddComponent<AddBuildingToDBTag>(entity);

                _bsEcb.RemoveComponent<PlaceTempBuildingTag>(entity);
                _bsEcb.RemoveComponent<TempBuildingTag>(entity);

                // add to buildings list for save mb
                gameBuildingsMap.Add(guid, building);

                //TODO add here tag for add building to db

                // Debug.Log("New building added");
            }
        }
    }
}