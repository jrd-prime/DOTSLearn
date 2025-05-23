﻿using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.UserInputAndCameraControl.CameraControl;
using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct DestroyTempBuildingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraData>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var bsEcb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (_, entity) in SystemAPI
                         .Query<TempBuildingTag>()
                         .WithAll<DestroyTempBuildingTag>()
                         .WithEntityAccess())
            {
                var cameraEntity = SystemAPI.GetSingletonEntity<CameraData>();

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
                BsEcb.AddComponent<MoveDirectionData>(CameraEntity);
                BsEcb.DestroyEntity(TempPrefabEntity);
            }
        }
    }
}