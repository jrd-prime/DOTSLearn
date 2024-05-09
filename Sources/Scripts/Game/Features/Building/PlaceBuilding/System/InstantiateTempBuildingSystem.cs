using System;
using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.Screen;
using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Unity.Burst;
using Unity.Collections;
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
            state.RequireForUpdate<InstantiateTempBlueprintData>();
            state.RequireForUpdate<BlueprintsBuffer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (query, entity) in SystemAPI
                         .Query<RefRO<InstantiateTempBlueprintData>>()
                         .WithAll<BuildingStateData>()
                         .WithEntityAccess())
            {
                var buffers = SystemAPI.GetSingletonBuffer<BlueprintsBuffer>();
                
                BlueprintsBuffer blueprintsBuffer = buffers[query.ValueRO.BlueprintId];

                FixedString64Bytes giud = Guid.NewGuid().ToString("N");

                var ecb = SystemAPI
                    .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

                var position = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsData>().Value;

                state.Dependency = new InstantiateTempBlueprintPrefabJob
                {
                    BuildingData = new BuildingData
                    {
                        Guid = giud,
                        Name = blueprintsBuffer.Name,
                        Prefab = blueprintsBuffer.Self,
                        BuildingEvents = new NativeQueue<BuildingEvent>(Allocator.Persistent),

                        NameId = blueprintsBuffer.NameId,
                        Level = blueprintsBuffer.Level,
                        ItemsPerHour = blueprintsBuffer.ItemsPerHour,
                        LoadCapacity = blueprintsBuffer.LoadCapacity,
                        MaxStorage = blueprintsBuffer.StorageCapacity
                    },
                    BsEcb = ecb,
                    BuildingStateEntity = entity,
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1
                }.Schedule(state.Dependency);
            }
        }

        [BurstCompile]
        private struct InstantiateTempBlueprintPrefabJob : IJob
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

                BsEcb.SetName(instantiate, "___ # Temp Blueprint Entity");

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

                BsEcb.RemoveComponent<InstantiateTempBlueprintData>(BuildingStateEntity);
            }
        }
    }
}