using Unity.Entities;

namespace JTimer.Component
{
    public struct TimerData : IComponentData
    {
        public Entity Self;
        public Entity Owner;
        public TimerType TimerType;
        public bool IsSet;
        public int Duration;
        public TimeWrapper StartTime;
        public TimeWrapper FinishTime;
        public TimeWrapper LastUpdate;
    }
}