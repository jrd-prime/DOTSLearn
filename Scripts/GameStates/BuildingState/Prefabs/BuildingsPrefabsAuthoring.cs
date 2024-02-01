using System;
using System.Collections.Generic;
using Jrd.Goods;
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

                var buildingBuffer = AddBuffer<BuildingsBuffer>(entity);
                var buildingRequiredItemsBuffer = AddBuffer<BuildingRequiredItemsBuffer>(entity);
                var buildingManufacturedItemsBuffer = AddBuffer<BuildingManufacturedItemsBuffer>(entity);

                foreach (BuildingDataSo buildingData in authoring._buildings)
                {
                    FillBuildingsBuffer(buildingData, buildingBuffer);
                    FillBuildingsItemsBuffer(buildingData.RequiredItems, buildingRequiredItemsBuffer);
                    FillBuildingsItemsBuffer(buildingData.ManufacturedItems, buildingManufacturedItemsBuffer);
                }

                AddComponent<JBuildingsPrefabsTag>(entity);
            }

            private void FillBuildingsBuffer(BuildingDataSo buildingData,
                DynamicBuffer<BuildingsBuffer> buildingBuffer)
            {
                buildingBuffer.Add(new BuildingsBuffer
                {
                    Self = GetEntity(buildingData.Prefab, TransformUsageFlags.Dynamic),
                    Name = buildingData.Name,
                    Size = buildingData.Size,
                    Category = buildingData.Category
                });
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
    }


    public struct JBuildingsPrefabsTag : IComponentData
    {
    }

    public struct BuildingsBuffer : IBufferElementData
    {
        public Entity Self;
        public FixedString64Bytes Name;
        public float2 Size;
        public BuildingCategory Category;
    }

    [Serializable]
    public struct BuildingRequiredItemsBuffer : IBufferElementData
    {
        public GoodsEnum _requiredItem;
        public int _count;
    }

    [Serializable]
    public struct BuildingManufacturedItemsBuffer : IBufferElementData
    {
        public GoodsEnum _manufacturedItem;
        public int _count;
    }
}