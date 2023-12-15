using Unity.Entities;

namespace Jrd.Build.old
{
    /// <summary>
    /// Префабы, из которых будем выбирать и размещать при переходе в режим строительства
    /// </summary>
    public struct TempBuildPrefabInstantiateComponent : IComponentData
    {
        public Entity tempBuildPrefab;
        public Entity instantiatedTempEntity;
        public bool IsPlaced;
    }
}