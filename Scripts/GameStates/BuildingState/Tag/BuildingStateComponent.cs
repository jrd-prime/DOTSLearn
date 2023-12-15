using Unity.Entities;

namespace Jrd.GameStates.BuildingState.Tag
{
    public struct BuildingStateComponent : IComponentData
    {
        public Entity SelectedPrefab;
        public int SelectedPrefabID;
    }
}