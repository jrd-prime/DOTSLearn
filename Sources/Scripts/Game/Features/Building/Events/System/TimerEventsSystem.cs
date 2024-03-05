using System;
using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.Timer;
using Sources.Scripts.Timer.System;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Events.System
{
    [UpdateAfter(typeof(JTimerSystem))]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct TimerEventsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            new TimerEventsJob { Time = new TimeWrapper(DateTime.Now), Ecb = ecb }.ScheduleParallel();
        }

        private partial struct TimerEventsJob : IJobEntity
        {
            public TimeWrapper Time;
            public EntityCommandBuffer.ParallelWriter Ecb;

            private void Execute([EntityIndexInQuery] int inQueryIndex, ref TimerData timer)
            {
                if (timer.FinishTime.Value.Ticks > Time.Value.Ticks) return;

                Debug.LogWarning(
                    $"JOB / Timer finished:{timer.TimerType} / {timer.Self} / {timer.Owner} / {timer.Duration}");

                switch (timer.TimerType)
                {
                    case TimerType.MoveToWarehouse:
                        Ecb.AddComponent(inQueryIndex, timer.Owner,
                            new AddEventToBuildingData { Value = BuildingEvent.MoveToWarehouse_Timer_Finished });
                        break;

                    case TimerType.OneLoadCycle:
                        Ecb.AddComponent(inQueryIndex, timer.Owner,
                            new AddEventToBuildingData { Value = BuildingEvent.Production_OneLoadCycle_Finished });
                        break;

                    case TimerType.FullLoadCycle:
                        Ecb.AddComponent(inQueryIndex, timer.Owner,
                            new AddEventToBuildingData { Value = BuildingEvent.Production_FullLoadCycle_Finished });
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ReturnTimerToPool(inQueryIndex, timer.Self);
            }

            //TODO timers entity pool
            private void ReturnTimerToPool(int inQueryIndex, Entity self) => Ecb.DestroyEntity(inQueryIndex, self);
        }
    }
}