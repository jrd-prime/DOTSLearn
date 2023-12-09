using Unity.Collections;
using Unity.Entities;

namespace Jrd.Build
{
    /// <summary>
    /// Префабы, из которых будем выбирать и размещать при переходе в режим строительства
    /// </summary>
    public struct TempBuildPrefabComponent : IComponentData
    {
    }

    public struct PrefabBufferElements : IBufferElementData
    {
        public Entity PrefabEntity;
    }
}