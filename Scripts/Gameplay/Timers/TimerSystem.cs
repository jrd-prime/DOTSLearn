using System;
using Unity.Burst;
using Unity.Entities;

namespace Jrd.Gameplay.Timers
{
    [BurstCompile]
    public partial struct TimerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new TimerJob { Time = DateTime.Now }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct TimerJob : IJobEntity
        {
            public DateTime Time;

            private void Execute(ref TimerData timerData)
            {
                if (!timerData.IsSet)
                {
                    timerData.StartTime = Time;
                    timerData.LastUpdate = Time;
                    timerData.FinishTime = Time.AddSeconds(timerData.Duration);
                    timerData.IsSet = true;
                }

                timerData.LastUpdate = Time;
            }
        }
    }
}