using Jrd.Build.old;
using Jrd.Build.Screen;
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
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (stateComponent, buildingComponent, entity) in SystemAPI
                         .Query<RefRO<BuildingStateComponent>, RefRO<PlaceBuildingComponent>>().WithEntityAccess())
            {
                Debug.Log(
                    $"{stateComponent.ValueRO.SelectedPrefab} + {buildingComponent.ValueRO.placePosition}");

                var tempPrefab = stateComponent.ValueRO.SelectedPrefab;

                if (tempPrefab == Entity.Null) return;

                var tempEntity = _ecb.Instantiate(tempPrefab);

                _ecb.SetComponent(tempEntity, new LocalTransform
                {
                    Position = buildingComponent.ValueRO.placePosition,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                _ecb.RemoveComponent<PlaceBuildingComponent>(entity);
            }

            // Destroy temp prefab if we exit from edit mode
            foreach (var query in SystemAPI.Query<RefRO<TempBuildPrefabInstantiateComponent>, TempPrefabForRemoveTag>()
                         .WithEntityAccess())
            {
                _ecb.DestroyEntity(query.Item1.ValueRO.instantiatedTempEntity);
                _ecb.RemoveComponent<TempPrefabForRemoveTag>(query.Item3);
            }
        }
    }
}