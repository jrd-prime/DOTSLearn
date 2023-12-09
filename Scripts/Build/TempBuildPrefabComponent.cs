using Unity.Entities;

namespace Jrd.Build
{
    /// <summary>
    /// Префаб, который будем размещать при переходе в режим строительства
    /// </summary>
    public struct TempBuildPrefabComponent : IComponentData
    {
        public Entity TempPrefab;
    }
}