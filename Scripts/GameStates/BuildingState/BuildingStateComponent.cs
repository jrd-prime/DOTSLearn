using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates.BuildingState
{
    public struct BuildingStateComponent : IComponentData
    {
        public NativeList<Entity> BuildingStateComponentEntities;
    }
}