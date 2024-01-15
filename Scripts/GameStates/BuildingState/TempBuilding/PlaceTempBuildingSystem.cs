using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.JUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    /// <summary>
    /// Place temp building prefab
    /// </summary>
    [BurstCompile]
    public partial struct PlaceTempBuildingSystem : ISystem
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ConfirmationPanelTag>();
            state.RequireForUpdate<GameBuildingsData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            Entity confirmationPanelEntity = SystemAPI.GetSingletonEntity<ConfirmationPanelTag>();

            NativeHashMap<FixedString64Bytes, BuildingData> gameBuildings =
                SystemAPI.GetSingletonRW<GameBuildingsData>().ValueRW.GameBuildings;

            //TODO replace init
            if (!gameBuildings.IsCreated)
                gameBuildings = new NativeHashMap<FixedString64Bytes, BuildingData>(1, Allocator.Persistent);

            foreach (var (buildingData, transform, entity) in SystemAPI
                         .Query<RefRW<BuildingData>, RefRO<LocalTransform>>()
                         .WithAll<PlaceTempBuildingTag, TempBuildingTag>()
                         .WithEntityAccess())
            {
                {
                    //TODO убрать
                    string guid = Utils.GetGuid();
                    Debug.LogWarning("build position = " + transform.ValueRO.Position);
                    buildingData.ValueRW.WorldPosition = transform.ValueRO.Position;
                    buildingData.ValueRW.Guid = guid;
                }

                var data = buildingData.ValueRO;

                gameBuildings.Add(data.Guid, new BuildingData
                {
                    Guid = data.Guid,
                    Name = data.Name,
                    Prefab = data.Prefab,
                    WorldPosition = data.WorldPosition
                });

                _bsEcb.SetName(entity, $"{data.Name}_{data.Guid}");

                Debug.LogWarning("New building added");

                _bsEcb.RemoveComponent<PlaceTempBuildingTag>(entity);
                _bsEcb.RemoveComponent<TempBuildingTag>(entity);

                // Hide confirmation panel
                _bsEcb.AddComponent<HideVisualElementTag>(confirmationPanelEntity);
            }
        }
    }
}