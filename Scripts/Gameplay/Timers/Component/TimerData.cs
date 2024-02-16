using System;
using Unity.Entities;

namespace Jrd.Gameplay.Timers
{
    public struct TimerData : IComponentData
    {
        public Entity Self;
        public Entity Owner;
        public TimerType TimerType;
        public bool IsSet;
        public int Duration;
        public DateTime StartTime;
        public DateTime FinishTime;
        public DateTime LastUpdate;
    }
}