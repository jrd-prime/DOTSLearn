using System.Collections.Generic;
using System.Linq;
using Jrd.ScriptableObjects;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.Building.Prefabs
{
    public class ProductsPrefabsAuthoring : MonoBehaviour
    {
        [SerializeField] public List<ProductDataSO> _products;


        private class Baker : Baker<ProductsPrefabsAuthoring>
        {
            public override void Bake(ProductsPrefabsAuthoring authoring)
            {
                Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                var buildingBuffer = AddBuffer<ProductsBuffer>(entity);
                // var buildingRequiredItemsBuffer = AddBuffer<BuildingRequiredItemsBuffer>(entity);
                // var buildingManufacturedItemsBuffer = AddBuffer<BuildingManufacturedItemsBuffer>(entity);

                foreach (ProductDataSO buildingData in authoring._products)
                {
                    FillBuildingsBuffer(buildingData, buildingBuffer);
                    // FillBuildingsItemsBuffer(buildingData.RequiredItems, buildingRequiredItemsBuffer);
                    // FillBuildingsItemsBuffer(buildingData.ManufacturedItems, buildingManufacturedItemsBuffer);
                }

                AddComponent<JProductsPrefabsTag>(entity);
            }

            private void FillBuildingsBuffer(ProductDataSO buildingData,
                DynamicBuffer<ProductsBuffer> buildingBuffer)
            {
                // buildingBuffer.Add(new ProductsBuffer
                // {
                //     Self = GetEntity(buildingData.Prefab, TransformUsageFlags.Dynamic),
                //
                //     // Building
                //     CategoryId = buildingData.CategoryId,
                //     NameId = buildingData.NameId,
                //     Name = buildingData.Name,
                //     Size = buildingData.Size,
                //
                //     // Stats
                //     Level = buildingData.Level,
                //     ItemsPerHour = buildingData.ItemsPerHour,
                //     LoadCapacity = buildingData.LoadCapacity,
                //     StorageCapacity = buildingData.StorageCapacity
                // });
            }

            private static void FillBuildingsItemsBuffer<T>(IReadOnlyList<T> list, DynamicBuffer<T> dynamicBuffer)
                where T : unmanaged, IBufferElementData
            {
                int count = list.Count;

                if (count == 0) return;

                for (var i = 0; i < count; i++)
                {
                    dynamicBuffer.Add(list[i]);
                }
            }
        }

        private void OnValidate()
        {
            if (_products.Count != _products.Distinct().Count())
            {
                Debug.LogError("Products list has duplicates!" + this);
            }
        }
    }

    public struct JProductsPrefabsTag : IComponentData
    {
    }

    public struct ProductsBuffer : IBufferElementData
    {
        public Entity Self;

        //     // Building
        //     public BuildingCategoryId CategoryId;
        //     public BuildingNameId NameId;
        //     public FixedString64Bytes Name;
        //     public float2 Size;
        //
        //     // Stats
        //     public int Level;
        //     public float ItemsPerHour;
        //     public int LoadCapacity;
        //     public int StorageCapacity;
    }

    // [Serializable]
    // public struct ProductRequiredItemsBuffer : IBufferElementData
    // {
    //     public Product _item;
    //     public int _count;
    // }
    //
    // [Serializable]
    // public struct BuildingManufacturedItemsBuffer : IBufferElementData
    // {
    //     public Product _item;
    //     public int _count;
    // }
}