using System;
using Jrd.Gameplay.Building;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Timers
{
    [UpdateAfter(typeof(TimerSystem))]
    [BurstCompile]
    public partial struct TimerEventsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            new TimerEventsJob { Time = DateTime.Now, Ecb = ecb }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct TimerEventsJob : IJobEntity
        {
            public DateTime Time;
            public EntityCommandBuffer.ParallelWriter Ecb;

            private void Execute([EntityIndexInQuery] int inQueryIndex, ref TimerData timer)
            {
                if (timer.FinishTime.Ticks > Time.Ticks) return;

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
            [BurstCompile]
            private void ReturnTimerToPool(int inQueryIndex, Entity self) => Ecb.DestroyEntity(inQueryIndex, self);
        }
    }
}