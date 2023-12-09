using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Build
{
    public class TempBuildPrefabAuthoring : MonoBehaviour
    {
        public List<GameObject> prefabs;

        public class TempBuildPrefabBaker : Baker<TempBuildPrefabAuthoring>
        {
            public override void Bake(TempBuildPrefabAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<PrefabBufferElements>(e);

                foreach (var prefab in authoring.prefabs)
                {
                    buffer.Add(new PrefabBufferElements
                    {
                        PrefabEntity = GetEntity(prefab, TransformUsageFlags.Dynamic)
                    });
                    AddComponent<TempBuildPrefabComponent>(e);
                }
            }
        }
    }
}