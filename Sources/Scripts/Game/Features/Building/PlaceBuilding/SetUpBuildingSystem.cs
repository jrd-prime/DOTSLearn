using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.Game.Features.Building.PlaceBuilding.Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding
{
    /// <summary>
    /// Place temp building prefab and init building
    /// </summary>
    [BurstCompile]
    public partial struct SetUpBuildingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlueprintsBlobData>();
            state.RequireForUpdate<PlaceTempBuildingTag>();
            state.RequireForUpdate<TempBuildingTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (building, transform, entity) in SystemAPI
                         .Query<RefRW<BuildingData>, RefRO<LocalTransform>>()
                         .WithAll<PlaceTempBuildingTag, TempBuildingTag>()
                         .WithEntityAccess())
            {
                EntityCommandBuffer bsEcb = SystemAPI
                    .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

                BlueprintsBlobData blueprintsBlob = SystemAPI.GetSingleton<BlueprintsBlobData>();

                new BuildingSetUp(building, entity, transform, blueprintsBlob, bsEcb).Initialize();
            }
        }
    }
}