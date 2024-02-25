using System;
using System.Collections.Generic;
using System.Linq;
using GamePlay.Building.SetUp;
using GamePlay.Products.Component;
using GamePlay.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using ProductData = GamePlay.Products.Component.ProductData;

namespace GamePlay.Prefabs
{
    public class BlueprintsShopPrefabsAuthoring : MonoBehaviour
    {
        public List<BuildingDataSo> _buildings;

        private class Baker : Baker<BlueprintsShopPrefabsAuthoring>
        {
            public override void Bake(BlueprintsShopPrefabsAuthoring authoring)
            {
                Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                AddComponent<BuildingsPrefabsBufferTag>(entity);

                var buff = AddBuffer<BlueprintsBuffer>(entity);

                int buildingsCount = authoring._buildings.Count;

                const int requiredCellId = 0;
                const int manufacturedCellId = 1;

                // req and man fixed 0 and 1 index
                // [][][] [building id][req or man cell id][product data]
                using BlobBuilder builder = new(Allocator.Temp);

                // ROOT
                ref BlobArray<BlobArray<BlobArray<ProductData>>> root =
                    ref builder.ConstructRoot<BlobArray<BlobArray<BlobArray<ProductData>>>>();

                // BUILDINGS ARRAY BUILDER
                BlobBuilderArray<BlobArray<BlobArray<ProductData>>> buildingsArray =
                    builder.Allocate(ref root, Enum.GetNames(typeof(BuildingNameId)).Length);

                for (int i = 0; i < buildingsCount; i++)
                {
                    int buildingId = (int)authoring._buildings[i].NameId;

                    // REQUIRED AND MANUFACTURED ARRAYS BUILDER
                    BlobBuilderArray<BlobArray<ProductData>> reqAndManArrays =
                        builder.Allocate(ref buildingsArray[buildingId], 2);

                    int requiredItemsCount = authoring._buildings[i].RequiredItems.Count;


                    // REQUIRED ARRAY BUILDER
                    BlobBuilderArray<ProductData> requiredArray =
                        builder.Allocate(ref reqAndManArrays[requiredCellId], requiredItemsCount);

                    if (requiredItemsCount != 0)
                    {
                        // REQ ARRAY
                        for (int j = 0; j < requiredItemsCount; j++)
                        {
                            requiredArray[j] = new ProductData
                            {
                                Name = authoring._buildings[i].RequiredItems[j]._productDataSo.Product,
                                Quantity = authoring._buildings[i].RequiredItems[j]._quantity
                            };
                        }
                    }

                    // MANUFACTURED ARRAY BUILDER
                    int manufacturedItemsCount = authoring._buildings[i].ManufacturedItems.Count;

                    BlobBuilderArray<ProductData> manufacturedArray =
                        builder.Allocate(ref reqAndManArrays[manufacturedCellId], manufacturedItemsCount);

                    if (manufacturedItemsCount != 0)
                    {
                        for (int j = 0; j < manufacturedItemsCount; j++)
                        {
                            manufacturedArray[j] = new ProductData
                            {
                                Name = authoring._buildings[i].ManufacturedItems[j]._productDataSo.Product,
                                Quantity = authoring._buildings[i].ManufacturedItems[j]._quantity
                            };
                        }
                    }
                }

                var blobAssetReference =
                    builder.CreateBlobAssetReference<BlobArray<BlobArray<BlobArray<ProductData>>>>(
                        Allocator.Persistent);

                AddComponent(entity, new BlueprintsBlobData
                {
                    Reference = blobAssetReference
                });

                foreach (var buildingData in authoring._buildings)
                {
                    buff.Add(new BlueprintsBuffer
                    {
                        Self = GetEntity(buildingData.Prefab, TransformUsageFlags.Dynamic),

                        CategoryId = buildingData.CategoryId,
                        NameId = buildingData.NameId,
                        Name = buildingData.Name,
                        Size = buildingData.Size,

                        Level = buildingData.Level,
                        ItemsPerHour = buildingData.ItemsPerHour,
                        LoadCapacity = buildingData.LoadCapacity,
                        StorageCapacity = buildingData.StorageCapacity
                    });
                }
            }
        }
    }

    public struct BlueprintsBlobData : IComponentData
    {
        public BlobAssetReference<BlobArray<BlobArray<BlobArray<ProductData>>>> Reference;

        public ProductionLineData GetProductionLineProducts(BuildingNameId buildingNameId)
        {
            ref BlobArray<ProductData> required = ref Reference.Value[(int)buildingNameId][0];
            ref BlobArray<ProductData> manufactured = ref Reference.Value[(int)buildingNameId][1];
            
            if (buildingNameId != BuildingNameId.Default)
            {
                return new ProductionLineData
                {
                    Required = GetProductsForBuildingFromReference(ref required),
                    Manufactured = GetProductsForBuildingFromReference(ref manufactured)
                };
            }

            throw new Exception($"Wrong building name id {nameof(BuildingNameId.Default)}!");
        }

        private static NativeList<ProductData> GetProductsForBuildingFromReference(ref BlobArray<ProductData> reference)
        {
            var data = new NativeList<ProductData>(0, Allocator.Persistent);

            if (reference.Length <= 0) return data;
            
            for (var i = 0; i < reference.Length; i++)
            {
                data.Add(reference[i]);
            }

            return data;
        }
    }

    public struct ProductionLineData
    {
        public NativeList<ProductData> Required;
        public NativeList<ProductData> Manufactured;
    }

    public struct BuildingsPrefabsBufferTag : IComponentData
    {
    }

    public struct BlueprintsBuffer : IBufferElementData
    {
        public Entity Self;

        public BuildingCategoryId CategoryId;
        public BuildingNameId NameId;
        public FixedString64Bytes Name;
        public float2 Size;

        public int Level;
        public float ItemsPerHour;
        public int LoadCapacity;
        public int StorageCapacity;
    }
}