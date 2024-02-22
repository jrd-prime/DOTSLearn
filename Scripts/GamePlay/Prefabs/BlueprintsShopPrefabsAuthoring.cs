using System.Collections.Generic;
using GamePlay.Building.SetUp;
using GamePlay.Products.Component;
using GamePlay.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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

                foreach (var buildingData in authoring._buildings)
                {
                    Debug.LogWarning("= = = = = = = = == = = = = = = == ");
                    Debug.LogWarning($"building {buildingData.NameId} /");
                    var req = new NativeList<ProductData>(0, Allocator.Persistent);
                    var man = new NativeList<ProductData>(0, Allocator.Persistent);

                    foreach (var requiredItem in buildingData.RequiredItems)
                    {
                        Debug.LogWarning($"req add = {requiredItem._productDataSo.Product} + {requiredItem._quantity}");
                        req.Add(new ProductData
                        {
                            Name = requiredItem._productDataSo.Product,
                            Quantity = requiredItem._quantity
                        });
                    }

                    foreach (var manufacturedItem in buildingData.ManufacturedItems)
                    {
                        Debug.LogWarning(
                            $"req add = {manufacturedItem._productDataSo.Product} + {manufacturedItem._quantity}");

                        man.Add(new ProductData
                        {
                            Name = manufacturedItem._productDataSo.Product,
                            Quantity = manufacturedItem._quantity
                        });
                    }

                    var e = GetEntity(buildingData.Prefab, TransformUsageFlags.Dynamic);
                    Debug.LogWarning($"e = {e}");
                    buff.Add(new BlueprintsBuffer
                    {
                        Self = e,

                        CategoryId = buildingData.CategoryId,
                        NameId = buildingData.NameId,
                        Name = buildingData.Name,
                        Size = buildingData.Size,

                        Level = buildingData.Level,
                        ItemsPerHour = buildingData.ItemsPerHour,
                        LoadCapacity = buildingData.LoadCapacity,
                        StorageCapacity = buildingData.StorageCapacity
                    });
                    AddComponent(e, new req { Value = req });
                    AddComponent(e, new man { Value = man });
                }
            }

            private static void FillBuffer<T>(List<ProductForProduction> data,
                DynamicBuffer<T> buffer) where T : unmanaged, ITes
            {
                foreach (var item in data)
                {
                    Debug.LogWarning($"add = {item._productDataSo.Product}");
                    var itemsBuffer = new T();

                    itemsBuffer.SetValue(new ProductData
                    {
                        Name = item._productDataSo.Product,
                        Quantity = item._quantity
                    });

                    buffer.Add(itemsBuffer);
                }
            }
        }
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

    public struct req : IComponentData
    {
        public NativeList<ProductData> Value;
    }

    public struct man : IComponentData
    {
        public NativeList<ProductData> Value;
    }

    public struct BuildingRequiredItemsBuffer : IBufferElementData, ITes
    {
        public ProductData Value;
        public void SetValue(ProductData value) => Value = value;
    }

    public struct BuildingManufacturedItemsBuffer : IBufferElementData, ITes
    {
        public ProductData Value;
        public void SetValue(ProductData value) => Value = value;
    }

    public struct BuildingProductionItemsBuffer
    {
        public ProductData Value;
    }

    internal interface ITes
    {
        public void SetValue(ProductData value);
    }
}