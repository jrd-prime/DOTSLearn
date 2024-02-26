using Unity.Collections;
using Unity.Entities;

namespace GamePlay.GameStates.BuildingState
{
    public struct BuildingStateComponent : IComponentData
    {
        public Entity Self;
        public bool IsInitialized;
        public NativeList<Entity> BuildingStateComponentEntities;
    }
}