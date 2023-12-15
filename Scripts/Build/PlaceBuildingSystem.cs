using Jrd.GameStates.BuildingState.Tag;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Build
{
    public partial struct PlaceBuildingSystem : ISystem
    {
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (stateComponent, place, entity) in SystemAPI
                         .Query<RefRW<BuildingStateComponent>, RefRO<PlaceBuildingComponent>>().WithEntityAccess())
            {
                Debug.Log($"PLACE : {place.ValueRO.placePrefab} + {place.ValueRO.placePosition}");

                var prefab = place.ValueRO.placePrefab;

                if (prefab == Entity.Null) return;

                var instantiate = _ecb.Instantiate(prefab);

                // stateComponent.ValueRW.TempEntity = instantiate;

                _ecb.SetComponent(entity, new BuildingStateComponent
                {
                    TempEntity = instantiate
                });
                
                _ecb.SetComponent(instantiate, new LocalTransform
                {
                    Position = place.ValueRO.placePosition,
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                _ecb.AddComponent(instantiate, new BuildingDetailsComponent
                {
                    entity = instantiate,
                    position = place.ValueRO.placePosition,
                    prefab = prefab
                });

                _ecb.RemoveComponent<PlaceBuildingComponent>(entity);
            }
        }
    }
}