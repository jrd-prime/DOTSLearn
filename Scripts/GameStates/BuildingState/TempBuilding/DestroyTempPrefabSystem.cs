﻿using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    [BurstCompile]
    public partial struct DestroyTempPrefabSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bsEcb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (_, entity) in SystemAPI
                         .Query<TempBuildingTag>().WithAll<DestroyTempPrefabTag>()
                         .WithEntityAccess())
            {
                var cameraEntity = SystemAPI.GetSingletonEntity<CameraComponent>();

                state.Dependency = new DestroyTempPrefabJob
                    {
                        BsEcb = bsEcb,
                        TempPrefabEntity = entity,
                        CameraEntity = cameraEntity
                    }
                    .Schedule(state.Dependency);
            }
        }

        [BurstCompile]
        private struct DestroyTempPrefabJob : IJob
        {
            public EntityCommandBuffer BsEcb;
            public Entity TempPrefabEntity;
            public Entity CameraEntity;

            public void Execute()
            {
                BsEcb.RemoveComponent<FollowComponent>(CameraEntity);
                BsEcb.AddComponent<MovingEventComponent>(CameraEntity);
                BsEcb.DestroyEntity(TempPrefabEntity);
            }
        }
    }
}