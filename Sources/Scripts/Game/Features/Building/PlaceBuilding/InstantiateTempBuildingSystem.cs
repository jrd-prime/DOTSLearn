using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.Game.Features.Building.PlaceBuilding.Component;
using Sources.Scripts.Screen;
using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding
{
    [BurstCompile]
    public partial struct InstantiateTempBuildingSystem : ISystem
    {
        private static readonly FixedString64Bytes TempBuildingEntityName = "___ # Temp Building Entity";

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<InstantiateTempBuildingData>();
            state.RequireForUpdate<ScreenCenterInWorldCoordsData>();
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

                ScreenCenterInWorldCoordsData coordsData = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsData>();

                state.Dependency = new InstantiateTempPrefabJob
                {
                    BuildingData = query.ValueRO.BuildingData,
                    BsEcb = ecb,
                    BuildingStateEntity = entity,
                    Position = coordsData.Value,
                    BuildingEntityName = TempBuildingEntityName
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
            public BuildingData BuildingData;
            public FixedString64Bytes BuildingEntityName;

            [BurstCompile]
            public void Execute()
            {
                Entity instantiate = BsEcb.Instantiate(BuildingData.Prefab);

                BuildingData.Self = instantiate;

                BsEcb.SetName(instantiate, BuildingEntityName);

                BsEcb.SetComponent(instantiate, new LocalTransform
                {
                    Position = Position,
                    Rotation = quaternion.identity,
                    Scale = 1
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