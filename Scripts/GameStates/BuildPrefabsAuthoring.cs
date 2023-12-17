using System.Collections.Generic;
using Jrd.GameStates;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Build.old
{
    public class BuildPrefabsAuthoring : MonoBehaviour
    {
        public List<GameObject> prefabs;

        public class Baker : Baker<BuildPrefabsAuthoring>
        {
            public override void Bake(BuildPrefabsAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<PrefabBufferElements>(e);

                foreach (var prefab in authoring.prefabs)
                {
                    buffer.Add(new PrefabBufferElements
                    {
                        PrefabEntity = GetEntity(prefab, TransformUsageFlags.Dynamic), PrefabName = prefab.name
                    });
                }

                AddComponent<BuildPrefabsComponent>(e);
            }
        }
    }
}