using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Jrd
{
    public class BuildingPrefabAuthoring : MonoBehaviour
    {
        public GameObject building1Prefab;
        public GameObject building2Prefab;

        public class BuildingPrefabBaker : Baker<BuildingPrefabAuthoring>
        {
            public override void Bake(BuildingPrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BuildingPrefabComponent
                {
                    Building1Prefab = GetEntity(authoring.building1Prefab, TransformUsageFlags.Dynamic),
                    Building2Prefab = GetEntity(authoring.building2Prefab, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new LocalTransform());
            }
        }
    }
}