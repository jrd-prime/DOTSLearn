using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.Screen;
using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct InstantiateTempBuildingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ScreenCenterInWorldCoordsData>();
            state.RequireForUpdate<InstantiateTempBuildingData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (query, entity) in SystemAPI
                         .Query<RefRO<InstantiateTempBuildingData>>()
                         .WithAll<BuildingStateData>()
                         .WithEntityAccess())
            {
                var ecb = SystemAPI
                    .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

                var position = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsData>().Value;
                
                state.Dependency = new InstantiateTempPrefabJob
                {
                    BuildingData = query.ValueRO.BuildingData,
                    BsEcb = ecb,
                    BuildingStateEntity = entity,
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1
                }.Schedule(state.Dependency);
            }
        }

        [BurstCompile]
        private struct InstantiateTempPrefabJob : IJob
        {
            public EntityCommandBuffer BsEcb;
            public BuildingNameId BuildingNameId;
            public Entity BuildingStateEntity;
            public float3 Position;
            public quaternion Rotation;
            public float Scale;
            public BuildingData BuildingData;

            [BurstCompile]
            public void Execute()
            {
                Entity instantiate = BsEcb.Instantiate(BuildingData.Prefab);
                BuildingData.Self = instantiate;
                BsEcb.SetName(instantiate, "___ # Temp Building Entity");

                BsEcb.SetComponent(instantiate, new LocalTransform
                {
                    Position = Position,
                    Rotation = Rotation,
                    Scale = Scale
                });

                BsEcb.AddComponent<TempBuildingTag>(instantiate);
                BsEcb.AddComponent<SelectableBuildingTag>(instantiate);
                BsEcb.AddComponent<MoveDirectionData>(instantiate);
                BsEcb.AddComponent(instantiate, BuildingData);

                BsEcb.RemoveComponent<InstantiateTempBuildingData>(BuildingStateEntity);
            }
        }
    }
}