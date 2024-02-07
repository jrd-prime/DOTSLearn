using Unity.Entities;

namespace Jrd.Gameplay.Timers
{
    public struct ProductsMoveTimerData : IComponentData
    {
        public float StarValue;
        public float CurrentValue;
    }
}