using System;
using System.Collections.Generic;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Authoring
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

                // [][][] [building id][req or man cell id][product data]
                using BlobBuilder builder = new(Allocator.Temp);

                // ROOT
                ref BlobArray<BlobArray<BlobArray<ProductData>>> root =
                    ref builder.ConstructRoot<BlobArray<BlobArray<BlobArray<ProductData>>>>();

                // BUILDINGS ARRAY BUILDER
                BlobBuilderArray<BlobArray<BlobArray<ProductData>>> buildingsArray =
                    builder.Allocate(ref root, Enum.GetNames(typeof(BuildingNameId)).Length);

                for (var i = 0; i < buildingsCount; i++)
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
                        for (var j = 0; j < requiredItemsCount; j++)
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
                        for (var j = 0; j < manufacturedItemsCount; j++)
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
}