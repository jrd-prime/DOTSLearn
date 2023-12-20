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
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (_, entity) in SystemAPI
                         .Query<TempBuildingTag>().WithAll<DestroyTempPrefabTag>()
                         .WithEntityAccess())
            {
                state.Dependency = new DestroyTempPrefabJob
                    {
                        BsEcb = ecb,
                        TempPrefabEntity = entity
                    }
                    .Schedule(state.Dependency);
            }
        }

        [BurstCompile]
        private struct DestroyTempPrefabJob : IJob
        {
            public EntityCommandBuffer BsEcb;
            public Entity TempPrefabEntity;

            public void Execute()
            {
                BsEcb.DestroyEntity(TempPrefabEntity);
            }
        }
    }
}