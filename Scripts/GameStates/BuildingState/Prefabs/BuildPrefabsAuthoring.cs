using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    public class BuildPrefabsAuthoring : MonoBehaviour
    {
        public List<GameObject> _prefabs;

        private class Baker : Baker<BuildPrefabsAuthoring>
        {
            public override void Bake(BuildPrefabsAuthoring authoring)
            {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                
                AddComponent<BuildPrefabsComponent>(entity);
                
                var buffer = AddBuffer<BuildingsPrefabsBuffer>(entity);

                foreach (var prefab in authoring._prefabs)
                {
                    buffer.Add(new BuildingsPrefabsBuffer
                    {
                        PrefabEntity = GetEntity(prefab, TransformUsageFlags.Dynamic),
                        PrefabName = prefab.name
                    });
                }

            }
        }
    }
}