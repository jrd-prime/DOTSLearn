using System;
using Sources.Scripts.Timer;
using Unity.Entities;
using UnityEngine;
using JTimerSystem = Sources.Scripts.Timer.System.JTimerSystem;

namespace Sources.Scripts.Game.Features.Building.Events
{
    [UpdateAfter(typeof(JTimerSystem))]
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
                    $"Timer finished:{timer.TimerType} / {timer.Self} / {timer.Owner} / {timer.Duration}");

                switch (timer.TimerType)
                {
                    case TimerType.MoveToWarehouse:
                        Ecb.AddComponent(inQueryIndex, timer.Owner,
                            new AddEventToBuildingData { Value = BuildingEvent.MoveToWarehouseTimerFinished });
                        break;

                    case TimerType.OneLoadCycle:
                        Ecb.AddComponent(inQueryIndex, timer.Owner,
                            new AddEventToBuildingData { Value = BuildingEvent.OneLoadCycleFinished });
                        break;

                    case TimerType.FullLoadCycle:
                        Ecb.AddComponent(inQueryIndex, timer.Owner,
                            new AddEventToBuildingData { Value = BuildingEvent.FullLoadCycleFinished });
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