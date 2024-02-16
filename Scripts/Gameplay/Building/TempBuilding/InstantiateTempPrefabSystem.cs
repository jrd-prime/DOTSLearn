using Jrd.Gameplay.Building.ControlPanel.Component;
using Jrd.Gameplay.Building.TempBuilding.Component;
using Jrd.GameStates.BuildingState;
using Jrd.Screen;
using Jrd.Select;
using Jrd.UserInput.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Gameplay.Building.TempBuilding
{
    [BurstCompile]
    public partial struct InstantiateTempPrefabSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenCenterInWorldCoordsData>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<InstantiateTempPrefabComponent>();
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

                BsEcb.AddComponent<TempBuildingTag>(instantiate); // mark as temp
                BsEcb.AddComponent<SelectableTag>(instantiate); // mark as selectable
                BsEcb.AddComponent<MoveDirectionData>(instantiate);
                BsEcb.AddComponent(instantiate, BuildingData);

                BsEcb.RemoveComponent<InstantiateTempPrefabComponent>(BuildingStateEntity);
            }
        }
    }

    public struct InstantiateTempPrefabComponent : IComponentData
    {
        public BuildingData BuildingData;
    }
}