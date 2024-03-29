﻿using System;
using System.Collections.Generic;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.SO;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Authoring
{
    public class BlueprintsShopPrefabsAuthoring : MonoBehaviour
    {
        public List<BuildingDataScriptable> _buildings;

        private class Baker : Baker<BlueprintsShopPrefabsAuthoring>
        {
            public override void Bake(BlueprintsShopPrefabsAuthoring authoring)
            {
                Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                AddComponent<BuildingsPrefabsBufferTag>(entity);

                var buff = AddBuffer<BlueprintsBuffer>(entity);

                int buildingsCount = authoring._buildings.Count;

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
                        builder.Allocate(ref reqAndManArrays[BlueprintsBlobAssetReference.RequiredCellId],
                            requiredItemsCount);

                    if (requiredItemsCount != 0)
                    {
                        NativeList<ProductData> tempArray = new NativeList<ProductData>(0, Allocator.Temp);
                        // REQ ARRAY
                        for (var j = 0; j < requiredItemsCount; j++)
                        {
                            tempArray.Add(new ProductData
                            {
                                Name = authoring._buildings[i].RequiredItems[j]._productDataScriptable.Product,
                                Quantity = authoring._buildings[i].RequiredItems[j]._quantity
                            });
                        }

                        if (requiredItemsCount == 2)
                        {
                            var sorted = SortMe(tempArray);

                            for (int j = 0; j < sorted.Length; j++)
                            {
                                requiredArray[j] = sorted[j];
                            }
                        }
                    }

                    // MANUFACTURED ARRAY BUILDER
                    int manufacturedItemsCount = authoring._buildings[i].ManufacturedItems.Count;

                    BlobBuilderArray<ProductData> manufacturedArray =
                        builder.Allocate(ref reqAndManArrays[BlueprintsBlobAssetReference.ManufacturedCellId],
                            manufacturedItemsCount);

                    if (manufacturedItemsCount != 0)
                    {
                        NativeList<ProductData> tempArray = new NativeList<ProductData>(0, Allocator.Temp);
                        for (var j = 0; j < manufacturedItemsCount; j++)
                        {
                            tempArray.Add(new ProductData
                            {
                                Name = authoring._buildings[i].ManufacturedItems[j]._productDataScriptable.Product,
                                Quantity = authoring._buildings[i].ManufacturedItems[j]._quantity
                            });
                        }

                        if (manufacturedItemsCount == 2)
                        {
                            var sorted = SortMe(tempArray);

                            for (int j = 0; j < sorted.Length; j++)
                            {
                                manufacturedArray[j] = sorted[j];
                            }
                        }
                    }
                }

                var blobAssetReference = builder
                    .CreateBlobAssetReference<BlobArray<BlobArray<BlobArray<ProductData>>>>(Allocator.Persistent);

                AddComponent(entity, new BlueprintsBlobAssetReference
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

            // TODO refact this shit
            private static NativeArray<ProductData> SortMe(NativeList<ProductData> requiredArray)
            {
                var nativeList = new NativeList<ProductData>(0, Allocator.Persistent);

                var temp = new[] { requiredArray[0].Name, requiredArray[1].Name };

                Array.Sort(temp);

                if (temp[0] == requiredArray[0].Name) return requiredArray.AsArray();

                nativeList.Add(requiredArray[1]);
                nativeList.Add(requiredArray[0]);

                return nativeList.ToArray(Allocator.Persistent);
            }
        }
    }
}