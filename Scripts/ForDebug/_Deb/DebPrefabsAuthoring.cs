using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Jrd.ForDebug._Deb
{
    public class DebPrefabsAuthoring : MonoBehaviour
    {
        public List<GameObject> prefabs;

        public class Baker : Baker<DebPrefabsAuthoring>
        {
            public override void Bake(DebPrefabsAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<DebPrefabBufferElements>(e);

                foreach (var prefab in authoring.prefabs)
                {
                    buffer.Add(new DebPrefabBufferElements
                    {
                        PrefabEntity = GetEntity(prefab, TransformUsageFlags.Dynamic),
                        PrefabName = prefab.name
                    });
                }

                // buffer.Clear();
                AddComponent<DebPrefabsComponent>(e);
            }
        }
    }
}