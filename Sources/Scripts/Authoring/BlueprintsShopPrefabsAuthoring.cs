using System;
using System.Collections.Generic;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.SO;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Authoring
{
    public class BlueprintsShopPrefabsAuthoring : MonoBehaviour
    {
        public List<BuildingDataScriptable> _blueprints;

        private class Baker : Baker<BlueprintsShopPrefabsAuthoring>
        {
            public override void Bake(BlueprintsShopPrefabsAuthoring authoring)
            {
                Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                AddComponent<BuildingsPrefabsBufferTag>(entity);

                var buff = AddBuffer<BlueprintsBuffer>(entity);

                int buildingsCount = authoring._blueprints.Count;

                // [][][] [building id][req or man cell id][product data]
                using BlobBuilder builder = new(Allocator.Temp);

                // ROOT
                ref BlobArray<BlobArray<BlobArray<ProductData>>> root =
                    ref builder.ConstructRoot<BlobArray<BlobArray<BlobArray<ProductData>>>>();

                // BUILDINGS ARRAY BUILDER
                BlobBuilderArray<BlobArray<BlobArray<ProductData>>> buildingsArray =
                    builder.Allocate(ref root, Enum.GetNames(typeof(BuildingNameId)).Length);

                Debug.LogWarning("bu name id length = " + Enum.GetNames(typeof(BuildingNameId)).Length);

                for (var i = 0; i < buildingsCount; i++)
                {
                    int buildingId = (int)authoring._blueprints[i].NameId;
                    // REQUIRED AND MANUFACTURED ARRAYS BUILDER
                    BlobBuilderArray<BlobArray<ProductData>> reqAndManArrays =
                        builder.Allocate(ref buildingsArray[buildingId], 2);

                    int requiredItemsCount = authoring._blueprints[i].RequiredItems.Count;

                    // REQUIRED ARRAY BUILDER
                    BlobBuilderArray<ProductData> requiredArray =
                        builder.Allocate(ref reqAndManArrays[BlueprintsBlobAssetReference.RequiredCellId],
                            requiredItemsCount);

                    if (requiredItemsCount != 0)
                    {
                        NativeArray<ProductData> tempArray = new NativeArray<ProductData>(requiredItemsCount, Allocator.Temp);
                        // REQ ARRAY
                        for (var j = 0; j < requiredItemsCount; j++)
                        {
                            var prod = new ProductData
                            {
                                Name = authoring._blueprints[i].RequiredItems[j]._productDataScriptable.Product,
                                Quantity = authoring._blueprints[i].RequiredItems[j]._quantity
                            };
                            Debug.LogWarning(tempArray.IsCreated);
                            tempArray[j] = prod;

                            // tempArray.Add(new ProductData
                            // {
                            //     Name = authoring._blueprints[i].RequiredItems[j]._productDataScriptable.Product,
                            //     Quantity = authoring._blueprints[i].RequiredItems[j]._quantity
                            // });
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
                    int manufacturedItemsCount = authoring._blueprints[i].ManufacturedItems.Count;

                    BlobBuilderArray<ProductData> manufacturedArray =
                        builder.Allocate(ref reqAndManArrays[BlueprintsBlobAssetReference.ManufacturedCellId],
                            manufacturedItemsCount);

                    if (manufacturedItemsCount != 0)
                    {
                        NativeArray<ProductData> tempArray =  new NativeArray<ProductData>(manufacturedItemsCount, Allocator.Temp);
                        for (var j = 0; j < manufacturedItemsCount; j++)
                        {
                            if (buildingId == 2)
                            {
                                Debug.LogWarning(
                                    $"i {i} , j {j} == {i}+{BlueprintsBlobAssetReference.ManufacturedCellId}+{j}");

                                Debug.LogWarning(">>> " + authoring._blueprints[i].ManufacturedItems[j]
                                    ._productDataScriptable.Product);
                            }

                            tempArray[j] = new ProductData
                            {
                                Name = authoring._blueprints[i].ManufacturedItems[j]._productDataScriptable.Product,
                                Quantity = authoring._blueprints[i].ManufacturedItems[j]._quantity
                            };
                            // tempArray.Add(new ProductData
                            // {
                            //     Name = authoring._blueprints[i].ManufacturedItems[j]._productDataScriptable.Product,
                            //     Quantity = authoring._blueprints[i].ManufacturedItems[j]._quantity
                            // });
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

                Debug.LogWarning("=== " + blobAssetReference.Value[2][1][0].Name);

                AddComponent(entity, new BlueprintsBlobAssetReference
                {
                    Reference = blobAssetReference
                });

                foreach (var buildingData in authoring._blueprints)
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
            private static NativeArray<ProductData> SortMe(NativeArray<ProductData> requiredArray)
            {
                var nativeArray = new NativeArray<ProductData>(requiredArray.Length, Allocator.Persistent);

                var temp = new[] { requiredArray[0].Name, requiredArray[1].Name };

                Array.Sort(temp);

                if (temp[0] == requiredArray[0].Name) return requiredArray;

                nativeArray[0] = requiredArray[1];
                nativeArray[1] = requiredArray[0];

                return nativeArray;
            }
        }

        // TODO made more informative or find another solution
        private void OnValidate()
        {
            var checkMap = new NativeHashMap<int, int>(_blueprints.Count, Allocator.Temp);

            foreach (var blueprint in _blueprints)
            {
                if (blueprint.NameId == BuildingNameId.Default) continue;

                Assert.IsFalse(checkMap.ContainsKey((int)blueprint.NameId),
                    $"Check blueprints SO Name ID. Name ID: {blueprint.NameId} exists on two or more SO.");

                checkMap.Add((int)blueprint.NameId, 0);
            }

            checkMap.Dispose();
        }
    }
}