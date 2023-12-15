using Jrd.Build.old;
using Jrd.Build.Screen;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Build
{
    public partial struct PlaceBuildingSystem : ISystem
    {
        private EntityManager _em;
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            _em = state.EntityManager;
            _ecb = new EntityCommandBuffer(Allocator.Temp);

            

            foreach (var (detailsComponent, placeBuildingComponent) in SystemAPI
                         .Query<RefRW<BuildingDetailsComponent>, RefRO<PlaceBuildingComponent>>())
            {
                Debug.Log(
                    $"finded building for place. {detailsComponent.ValueRO.prefab} + {placeBuildingComponent.ValueRO.placePosition}");

                var tempPrefab = detailsComponent.ValueRO.prefab;

                if (tempPrefab == Entity.Null) return;

                var tempEntity = detailsComponent.ValueRO.entity;

                _ecb.SetComponent(tempEntity, new LocalTransform
                {
                    Position = placeBuildingComponent.ValueRO.placePosition,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                // _ecb.SetComponent(entity, new TempBuildPrefabInstantiateComponent
                // {
                //     tempBuildPrefab = tempPrefab,
                //     instantiatedTempEntity = tempEntity
                // });
                //
                // _ecb.RemoveComponent<TempPrefabForPlaceTag>(entity);
            }

            // Destroy temp prefab if we exit from edit mode
            foreach (var query in SystemAPI.Query<RefRO<TempBuildPrefabInstantiateComponent>, TempPrefabForRemoveTag>()
                         .WithEntityAccess())
            {
                _ecb.DestroyEntity(query.Item1.ValueRO.instantiatedTempEntity);
                _ecb.RemoveComponent<TempPrefabForRemoveTag>(query.Item3);
            }


            _ecb.Playback(_em);
            _ecb.Dispose();
        }
    }
}