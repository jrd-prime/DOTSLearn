using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    [BurstCompile]
    public partial struct InstantiateTempPrefabSystem : ISystem
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

            foreach (var (instantiateTempPrefabComponent, entity) in SystemAPI
                         .Query<RefRO<InstantiateTempPrefabComponent>>()
                         .WithEntityAccess())
            {
                state.Dependency = new InstantiateTempPrefabJob
                    {
                        TempPrefabEntity = instantiateTempPrefabComponent.ValueRO.Prefab,
                        BsEcb = ecb,
                        BuildingStateEntity = entity
                    }
                    .Schedule(state.Dependency);
            }
        }

        [BurstCompile]
        private struct InstantiateTempPrefabJob : IJob
        {
            public EntityCommandBuffer BsEcb;
            public Entity TempPrefabEntity;
            public Entity BuildingStateEntity;

            public void Execute()
            {
                // instantiate selected building prefab
                var instantiate = BsEcb.Instantiate(TempPrefabEntity);

                // set position // TODO
                BsEcb.SetComponent(instantiate,
                    new LocalTransform { Position = new float3(3, 0, 5), Rotation = quaternion.identity, Scale = 1 });

                // add tag to instantiated prefab
                BsEcb.AddComponent<TempBuildingTag>(instantiate);

                // remove tag fo instantiate from building mode entity
                BsEcb.RemoveComponent<InstantiateTempPrefabComponent>(BuildingStateEntity);
            }
        }
    }
}