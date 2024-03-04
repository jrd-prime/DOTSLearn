using System;
using Sources.Scripts.CommonData;
using Unity.Entities;

namespace Sources.Scripts.Timer.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct JTimerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            new TimerJob { Time = new TimeWrapper(DateTime.Now) }.ScheduleParallel();
        }

        private partial struct TimerJob : IJobEntity
        {
            public TimeWrapper Time;

            private void Execute(ref TimerData timerData)
            {
                if (!timerData.IsSet)
                {
                    timerData.StartTime = Time;
                    timerData.LastUpdate = Time;
                    timerData.FinishTime = new TimeWrapper(Time.Value.AddSeconds(timerData.Duration));
                    timerData.IsSet = true;
                }

                timerData.LastUpdate = Time;
            }
        }
    }
}