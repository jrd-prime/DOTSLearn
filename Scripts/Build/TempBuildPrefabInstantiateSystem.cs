﻿using System.Runtime.CompilerServices;
using Jrd.Build.EditModePanel;
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
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial struct TempBuildPrefabInstantiateSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;

        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TempBuildPrefabInstantiateSystem");
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();

            _ecb = new EntityCommandBuffer(Allocator.Temp, PlaybackPolicy.SinglePlayback);
            _em = state.EntityManager;

            // state.EntityManager.AddComponent<TempBuildPrefabComponent>(state.SystemHandle);

            // var e = _ecb.CreateEntity();
            // _ecb.SetName(e, "_Entity_TempBuildPrefab");
            // _ecb.AddComponent<TempBuildPrefabComponent>(e);
            // _ecb.Playback(_em);
            // _ecb.Dispose();
        }

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
                // ecb.SetComponent(entity, new TempBuildPrefabComponent
                // {
                //     tempBuildPrefab = tempPrefab,
                //     instantiatedTempEntity = tempEntity
                // });

                ecb.RemoveComponent<TempPrefabForPlaceTag>(entity);
            }

            // Destroy temp prefab if we exit from edit mode
            foreach (var query in SystemAPI.Query<RefRO<TempBuildPrefabComponent>, TempPrefabForRemoveTag>()
                         .WithEntityAccess())
            {
                ecb.DestroyEntity(query.Item1.ValueRO.instantiatedTempEntity);
                // ecb.RemoveComponent<TempPrefabForRemoveTag>(query.Item3);
            }

            ecb.Playback(em);
            ecb.Dispose();
        }
    }
}