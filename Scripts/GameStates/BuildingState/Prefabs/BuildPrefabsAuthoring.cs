using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    public class BuildPrefabsAuthoring : MonoBehaviour
    {
        // public List<GameObject> _prefabs;
        public List<BuildingDataSO> _buildings;

        private class Baker : Baker<BuildPrefabsAuthoring>
        {
            public override void Bake(BuildPrefabsAuthoring authoring)
            {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

                AddComponent<BuildPrefabsComponent>(entity);

                var buffer = AddBuffer<BuildingsBuffer>(entity);

                foreach (var prefab in authoring._buildings)
                {
                    buffer.Add(new BuildingsBuffer
                    {
                        Self = GetEntity(prefab.Prefab, TransformUsageFlags.Dynamic),
                        Name = prefab.Name,
                        Size = prefab.Size,
                        Category = prefab.Category
                    });
                }
            }
        }
    }

    public struct BuildPrefabsComponent : IComponentData
    {
        public Entity BuildPrefab;
    }

    public struct BuildingsBuffer : IBufferElementData
    {
        public Entity Self;
        public FixedString64Bytes Name;
        public float2 Size;
        public BuildingCategory Category;
    }
}