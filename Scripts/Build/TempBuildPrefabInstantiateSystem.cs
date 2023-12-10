using Jrd.Build.Screen;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Build
{
    /// <summary>
    /// Размещаем тэмп префаб
    /// </summary>
    public partial struct TempBuildPrefabInstantiateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();
        }

        // LOOK гдет надо устанавливать префаб который хотим плэйсить
        // типа надо гдето выбирать конкретный префаб, создавать компонент, который тут будем ловить
        // и сэтить в него префаб, а тут только плэйс

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var em = state.EntityManager;
            var coordsForPlace = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsComponent>().ScreenCenterToWorld;

            // Instantiate temp prefab
            foreach (var (prefab, tag, entity) in SystemAPI
                         .Query<RefRW<TempBuildPrefabComponent>, TempPrefabForPlaceTag>()
                         .WithEntityAccess())
            {
                var tempPrefab = prefab.ValueRO.tempBuildPrefab;

                if (tempPrefab == Entity.Null) return;

                var tempEntity = ecb.Instantiate(tempPrefab);

                ecb.SetComponent(tempEntity, new LocalTransform
                {
                    Position = coordsForPlace,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                ecb.SetComponent(entity, new TempBuildPrefabComponent
                {
                    tempBuildPrefab = tempPrefab,
                    instantiatedTempEntity = tempEntity
                });

                ecb.RemoveComponent<TempPrefabForPlaceTag>(entity);
            }

            // Destroy temp prefab if we exit from edit mode
            foreach (var query in SystemAPI.Query<RefRO<TempBuildPrefabComponent>, TempPrefabForRemoveTag>()
                         .WithEntityAccess())
            {
                Debug.Log("founded remove tag");
                ecb.DestroyEntity(query.Item1.ValueRO.instantiatedTempEntity);
                ecb.RemoveComponent<TempPrefabForRemoveTag>(query.Item3);
            }


            ecb.Playback(em);
            ecb.Dispose();
        }
    }
}

// foreach (var (mapBuffer, entity) in SystemAPI.Query<DynamicBuffer<MapElement>>().WithAll<MapEntities>()
//              .WithEntityAccess())
//         {
//             for (int i = 0; i < mapBuffer.Length; i++)
//             {
//                 state.EntityManager.Instantiate(mapBuffer[i].MapEntity);
//             }
//         }