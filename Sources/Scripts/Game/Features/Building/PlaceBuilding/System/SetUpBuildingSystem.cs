using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.System
{
    /// <summary>
    /// Place temp building prefab and init building
    /// </summary>
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct SetUpBuildingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlueprintsBlobAssetReference>();
            state.RequireForUpdate<PlaceTempBuildingTag>();
            state.RequireForUpdate<TempBuildingTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

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

                BlueprintsBlobAssetReference blueprintsBlob = SystemAPI.GetSingleton<BlueprintsBlobAssetReference>();

                BuildingNameId buildingId = building.ValueRO.NameId;

                NativeList<ProductData> requiredItems =
                    blueprintsBlob.GetProductionLineProducts(buildingId).RequiredPtr;
                NativeList<ProductData> manufacturedItems =
                    blueprintsBlob.GetProductionLineProducts(buildingId).ManufacturedPtr;

                BuildingSetUpDataWrapper buildingSetUpDataWrapper = new()
                {
                    BuildingData = building,
                    Entity = entity,
                    Transform = transform,
                    RequiredItemsList = requiredItems,
                    ManufacturedItemsList = manufacturedItems,
                    Ecb = bsEcb
                };

                new BuildingSetUp(buildingSetUpDataWrapper).Initialize();
            }
        }
    }
}