using Jrd.GameplayBuildings;
using Jrd.Screen;
using Jrd.UserInput;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

            foreach (var (query, entity) in SystemAPI
                         .Query<RefRO<InstantiateTempPrefabComponent>>()
                         .WithAll<BuildingStateData>()
                         .WithEntityAccess())
            {
                state.Dependency = new InstantiateTempPrefabJob
                    {
                        TempPrefabEntity = query.ValueRO.Prefab,
                        TempPrefabName = query.ValueRO.Name,
                        BuildingNameId = query.ValueRO.NameId,
                        BsEcb = ecb,
                        BuildingStateEntity = entity,
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
            public FixedString64Bytes TempPrefabName;
            public BuildingNameId BuildingNameId;
            public Entity BuildingStateEntity;
            public float3 Position;
            public quaternion Rotation;
            public float Scale;

            [BurstCompile]
            public void Execute()
            {
                Entity instantiate = BsEcb.Instantiate(TempPrefabEntity);

                BsEcb.SetComponent(instantiate, new LocalTransform
                {
                    Position = Position,
                    Rotation = Rotation,
                    Scale = Scale
                });

                BsEcb.SetName(instantiate, "___ # Temp Building Entity");

                // add tags
                BsEcb.AddComponent<TempBuildingTag>(instantiate); // mark as temp
                BsEcb.AddComponent<SelectableTag>(instantiate); // mark as selectable
                BsEcb.AddComponent<MoveDirectionData>(instantiate);
                BsEcb.AddComponent(instantiate, new BuildingData
                {
                    Prefab = TempPrefabEntity,
                    Name = TempPrefabName,
                    NameId = BuildingNameId
                });

                BsEcb.RemoveComponent<InstantiateTempPrefabComponent>(BuildingStateEntity);
            }
        }
    }
}