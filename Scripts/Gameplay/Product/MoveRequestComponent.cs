using Unity.Entities;

namespace Jrd.Gameplay.Product
{
    /// <summary>
    /// Move products to/from main storage
    /// </summary>
    public struct MoveRequestComponent : IComponentData
    {
        public Entity Value;
    }
}