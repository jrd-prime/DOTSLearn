using Unity.Entities;

namespace Jrd
{
    public struct TempBuildingPrefabTag : IComponentData
    {
        public Entity TempEntity;
    }
}