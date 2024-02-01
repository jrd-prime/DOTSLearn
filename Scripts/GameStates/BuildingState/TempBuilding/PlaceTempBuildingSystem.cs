using Jrd.GameplayBuildings;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    /// <summary>
    /// Place temp building prefab
    /// </summary>
    // [BurstCompile]
    public partial struct PlaceTempBuildingSystem : ISystem
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;

        // [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlaceTempBuildingTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameBuildingsData>();
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsMap = SystemAPI
                .GetSingletonRW<GameBuildingsData>().ValueRW.GameBuildings;

            foreach (var (buildingData, transform, entity) in SystemAPI
                         .Query<RefRW<BuildingData>, RefRO<LocalTransform>>()
                         .WithAll<PlaceTempBuildingTag, TempBuildingTag>()
                         .WithEntityAccess())
            {
                string guid = Utils.Utils.GetGuid();
                buildingData.ValueRW.Self = entity;
                buildingData.ValueRW.WorldPosition = transform.ValueRO.Position;
                buildingData.ValueRW.Guid = guid;
                buildingData.ValueRW.Level = 1;
                buildingData.ValueRW.LoadCapacity = 20;
                buildingData.ValueRW.MaxStorage = 10;
                buildingData.ValueRW.Speed = 5;

                _bsEcb.SetName(entity, $"{buildingData.ValueRO.Name}_{guid}");
                _bsEcb.AddComponent<BuildingTag>(entity);
                _bsEcb.AddComponent<AddBuildingToDBTag>(entity);

                Debug.Log("New building added");

                _bsEcb.RemoveComponent<PlaceTempBuildingTag>(entity);
                _bsEcb.RemoveComponent<TempBuildingTag>(entity);

                //TODO to new system, add tag for add to game buildings map or db
                var data = buildingData.ValueRO;
                gameBuildingsMap.Add(data.Guid, new BuildingData
                {
                    Guid = data.Guid,
                    Self = entity,
                    Name = data.Name,
                    Prefab = data.Prefab,
                    WorldPosition = data.WorldPosition,
                    NameId = data.NameId,
                    
                    // TODO from build settings
                    Level = 1,
                    LoadCapacity = 20,
                    MaxStorage = 10,
                    Speed = 5
                });
            }
        }
    }
}