using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    public struct InstantiateTempPrefabComponent : IComponentData
    {
        public Entity Prefab;
        public FixedString64Bytes Name;
    }
}