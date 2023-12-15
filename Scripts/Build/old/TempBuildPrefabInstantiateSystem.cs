using Jrd.Build.Screen;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Build.old
{
    /// <summary>
    /// Размещаем тэмп префаб
    /// </summary>

    [UpdateBefore(typeof(BuildSystem))]
    public partial struct TempBuildPrefabInstantiateSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;

        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TempBuildPrefabInstantiateSystem");
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();

            _ecb = new EntityCommandBuffer(Allocator.Temp);
            _em = state.EntityManager;

            _ecb.AddComponent<TempBuildPrefabInstantiateComponent>(_ecb.CreateEntity());
            _ecb.Playback(_em);
            _ecb.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp);

            var coordsForPlace = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsComponent>().ScreenCenterToWorld;

            // Instantiate temp prefab
            foreach (var (prefab, tag, entity) in SystemAPI
                         .Query<RefRW<TempBuildPrefabInstantiateComponent>, TempPrefabForPlaceTag>()
                         .WithEntityAccess())
            {
                var tempPrefab = prefab.ValueRO.tempBuildPrefab;

                if (tempPrefab == Entity.Null) return;

                var tempEntity = _ecb.Instantiate(tempPrefab);

                _ecb.SetComponent(tempEntity, new LocalTransform
                {
                    Position = coordsForPlace,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                _ecb.SetComponent(entity, new TempBuildPrefabInstantiateComponent
                {
                    tempBuildPrefab = tempPrefab,
                    instantiatedTempEntity = tempEntity
                });

                _ecb.RemoveComponent<TempPrefabForPlaceTag>(entity);
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