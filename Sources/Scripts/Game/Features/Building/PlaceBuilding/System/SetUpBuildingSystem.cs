using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Production;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

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
                Debug.LogWarning("SETUP BUILDING " + building.ValueRO.Name);

                EntityCommandBuffer bsEcb = SystemAPI
                    .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

                BlueprintsBlobAssetReference blueprintsBlob = SystemAPI.GetSingleton<BlueprintsBlobAssetReference>();

                BuildingNameId buildingId = building.ValueRO.NameId;

                Debug.LogWarning(buildingId);


                ProductionLineData prodLine = blueprintsBlob.GetProductionLineProducts(buildingId);

                // Debug.LogWarning("requiredItems");
                // NativeList<ProductData> requiredItems =
                //     blueprintsBlob.GetProductionLineProducts(buildingId).Required;
                // Debug.LogWarning("requiredItems END");
                //
                // Debug.LogWarning("manufacturedItems");
                // NativeList<ProductData> manufacturedItems =
                //     blueprintsBlob.GetProductionLineProducts(buildingId).Manufactured;
                // Debug.LogWarning("manufacturedItems END");

                NativeList<ProductData> requiredItems = prodLine.Required;
                NativeList<ProductData> manufacturedItems = prodLine.Manufactured;


                for (int i = 0; i < blueprintsBlob.Reference.Value.Length; i++)
                {
                    Debug.LogWarning("bu id = " + i);
                    
                    
                    Debug.LogWarning("req");
                    for (int k = 0; k < blueprintsBlob.Reference.Value[i][0].Length; k++)
                    {
                        Debug.LogWarning(blueprintsBlob.Reference.Value[i][0][k].Name);
                    }

                    Debug.LogWarning("man");
                    for (int k = 0; k < blueprintsBlob.Reference.Value[i][1].Length; k++)
                    {
                        Debug.LogWarning(blueprintsBlob.Reference.Value[i][1][k].Name);
                    }
                }

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