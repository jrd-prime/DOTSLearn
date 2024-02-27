using System;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Timer
{
    public struct JTimer
    {
        /// <summary>
        /// Start new timer by creating new entity with component <see cref="TimerData"/>
        /// </summary>
        public void StartNewTimer(Entity owner, TimerType timerType, int duration, EntityCommandBuffer ecb)
        {
            Assert.IsTrue(owner != Entity.Null, "Owner entity is null");
            Assert.IsTrue(duration > 0, "Duration <= 0");
            Assert.IsTrue(ecb.IsCreated, "EntityCommandBuffer not created");

            //TODO create entity pool
            Entity timerEntity = ecb.CreateEntity();
            string guid = Guid.NewGuid().ToString("N");

            ecb.SetName(timerEntity, "TimerEntity_" + guid);

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