using System.Collections.Generic;
using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Products.Component;
using Jrd.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    public class BuildingsPrefabsAuthoring : MonoBehaviour
    {
        public List<BuildingDataSo> _buildings;

        private class Baker : Baker<BuildingsPrefabsAuthoring>
        {
            public override void Bake(BuildingsPrefabsAuthoring authoring)
            {
                Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);
                AddComponent<BuildingsPrefabsBufferTag>(entity);

                var buildingBuffer = AddBuffer<BlueprintsBuffer>(entity);
                var buildingRequiredItemsBuffer = AddBuffer<BuildingRequiredItemsBuffer>(entity);
                var buildingManufacturedItemsBuffer = AddBuffer<BuildingManufacturedItemsBuffer>(entity);


                foreach (BuildingDataSo buildingData in authoring._buildings)
                {
                    FillBuildingsBuffer(buildingData, buildingBuffer);

                    int count = buildingData.RequiredItems.Count;

                    if (count == 0) return;

                    for (var i = 0; i < count; i++)
                    {
                        buildingRequiredItemsBuffer.Add(new BuildingRequiredItemsBuffer
                        {
                            Value = new ProductData
                            {
                                Name = buildingData.RequiredItems[i]._productDataSo.Product,
                                Quantity = buildingData.RequiredItems[i]._quantity
                            }
                        });
                    }

                    int count1 = buildingData.ManufacturedItems.Count;

                    if (count1 == 0) return;

                    for (var i = 0; i < count1; i++)
                    {
                        buildingManufacturedItemsBuffer.Add(new BuildingManufacturedItemsBuffer()
                        {
                            Value = new ProductData()
                            {
                                Name = buildingData.ManufacturedItems[i]._productDataSo.Product,
                                Quantity = buildingData.ManufacturedItems[i]._quantity
                            }
                        });
                    }
                }
            }

            private void FillBuildingsBuffer(BuildingDataSo buildingData,
                DynamicBuffer<BlueprintsBuffer> buildingBuffer)
            {
                buildingBuffer.Add(new BlueprintsBuffer
                {
                    Self = GetEntity(buildingData.Prefab, TransformUsageFlags.Dynamic),

                    // Building
                    CategoryId = buildingData.CategoryId,
                    NameId = buildingData.NameId,
                    Name = buildingData.Name,
                    Size = buildingData.Size,

                    // Stats
                    Level = buildingData.Level,
                    ItemsPerHour = buildingData.ItemsPerHour,
                    LoadCapacity = buildingData.LoadCapacity,
                    StorageCapacity = buildingData.StorageCapacity
                });
            }
        }
    }

    public struct BuildingsPrefabsBufferTag : IComponentData
    {
    }

    public struct BlueprintsBuffer : IBufferElementData
    {
        public Entity Self;

        // Building
        public BuildingCategoryId CategoryId;
        public BuildingNameId NameId;
        public FixedString64Bytes Name;
        public float2 Size;

        // Stats
        public int Level;
        public float ItemsPerHour;
        public int LoadCapacity;
        public int StorageCapacity;
    }

    public struct BuildingRequiredItemsBuffer : IBufferElementData
    {
        public ProductData Value;
    }

    public struct BuildingManufacturedItemsBuffer : IBufferElementData
    {
        public ProductData Value;
    }

    public struct BuildingProductionItemsBuffer
    {
        public ProductData Value;
    }
}