using Jrd.Screen;
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
            state.RequireForUpdate<ScreenCenterInWorldCoordsData>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            var position = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsData>().ScreenCenterToWorld;

            foreach (var query in SystemAPI
                         .Query<RefRO<InstantiateTempPrefabComponent>>()
                         .WithEntityAccess())
            {
                state.Dependency = new InstantiateTempPrefabJob
                    {
                        TempPrefabEntity = query.Item1.ValueRO.Prefab,
                        BsEcb = ecb,
                        BuildingStateEntity = query.Item2,
                        Position = position,
                        Rotation = quaternion.identity,
                        Scale = 1
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
            public float3 Position;
            public quaternion Rotation;
            public float Scale;

            public void Execute()
            {
                // instantiate selected building prefab
                var instantiate = BsEcb.Instantiate(TempPrefabEntity);

                // set position // TODO
                BsEcb.SetComponent(instantiate,
                    new LocalTransform
                    {
                        Position = Position,
                        Rotation = Rotation,
                        Scale = Scale
                    });

                // name
                BsEcb.SetName(instantiate, "___ # Temp Building Entity");

                // add tag to instantiated prefab
                BsEcb.AddComponent<TempBuildingTag>(instantiate);

                // remove tag fo instantiate from building mode entity
                BsEcb.RemoveComponent<InstantiateTempPrefabComponent>(BuildingStateEntity);
            }
        }
    }
}