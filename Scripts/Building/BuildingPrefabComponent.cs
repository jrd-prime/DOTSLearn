using Unity.Entities;

namespace Jrd
{
    public struct BuildingPrefabComponent : IComponentData
    {
        public Entity Building1Prefab;
        public Entity Building2Prefab;
    }
}