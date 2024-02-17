using Jrd.MyUtils;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Timers
{
    public partial struct JTimer : ISystem
    {
        public void StartNewTimer(Entity owner, TimerType timerType, int duration, EntityCommandBuffer ecb)
        {
            //TODO create entity pool
            var timerEntity = ecb.CreateEntity();

            ecb.SetName(timerEntity, "TimerEntity_" + Utils.GetGuid());

            ecb.AddComponent(timerEntity, new TimerData
            {
                Self = timerEntity,
                Owner = owner,
                TimerType = timerType,
                IsSet = false,
                Duration = duration
            });

            Debug.Log("___ TIMER. New timer started. Move time: " + duration + " sec.");
        }
    }
}