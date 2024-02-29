using Sources.Scripts.CommonComponents;
using Sources.Scripts.UserInputAndCameraControl.CameraControl;
using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Unity.Burst;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct DestroyTempBuildingSystem : ISystem
    {
        private EntityQuery _tempBuildingForDestroyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CameraData>();

            _tempBuildingForDestroyQuery = state.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TempBuildingTag>(),
                ComponentType.ReadOnly<DestroyTempBuildingTag>());

            state.RequireForUpdate(_tempBuildingForDestroyQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer bsEcb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            Entity cameraEntity = SystemAPI.GetSingletonEntity<CameraData>();

            state.Dependency = new DestroyTempBuildingJob
                {
                    BsEcb = bsEcb,
                    CameraEntity = cameraEntity
                }
                .Schedule(_tempBuildingForDestroyQuery, state.Dependency);
        }

        [BurstCompile]
        public partial struct DestroyTempBuildingJob : IJobEntity
        {
            public EntityCommandBuffer BsEcb;
            public Entity CameraEntity;

            public void Execute(Entity entity)
            {
                BsEcb.RemoveComponent<FollowComponent>(CameraEntity);
                BsEcb.AddComponent<MoveDirectionData>(CameraEntity);
                BsEcb.DestroyEntity(entity);
            }
        }
    }
}