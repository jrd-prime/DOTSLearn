using Jrd.GameStates.BuildingState;
using Jrd.GameStates.BuildingState.Tag;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Build
{
    public partial struct PlaceBuildingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (place, entity) in SystemAPI
                         .Query<RefRO<PlaceBuildingComponent>>().WithAll<BuildingStateComponent>().WithEntityAccess())
            {
                Debug.Log($"PLACE : {place.ValueRO.placePrefab} + {place.ValueRO.placePosition}");

                var prefab = place.ValueRO.placePrefab;

                if (prefab == Entity.Null) return;

                var instantiate = ecb.Instantiate(prefab);

                ecb.SetComponent(instantiate, new LocalTransform
                {
                    Position = place.ValueRO.placePosition,
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                ecb.SetName(instantiate, "bl");

                ecb.AddComponent(instantiate, new BuildingDetailsComponent
                {
                    entity = instantiate,
                    position = place.ValueRO.placePosition,
                    prefab = prefab
                });
                ecb.AddComponent<TempBuildingTag>(instantiate);

                ecb.RemoveComponent<PlaceBuildingComponent>(entity);
            }
        }
    }
}