using Jrd.Gameplay.Building;
using Jrd.Gameplay.Timers.Component;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage.Warehouse
{
    /// <summary>
    /// Do stuff on timer finished
    /// </summary>
    public partial struct MoveToWarehouseTimerFinishedSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoveToWarehouseTimerFinishedEvent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<MoveToWarehouseTimerFinishedEvent>())
            {
                Debug.LogWarning("timer finished for move to warehouse");
            }
        }
    }
}