using Jrd.JCamera;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.Build
{
    /// <summary>
    /// Получаем мировые координаты точки с центра экрана
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial struct ScreenCenterToWorldSystem : ISystem
    {
        private Entity _screenCenterToWorldSingleton;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenComponent>();
            state.RequireForUpdate<ScreenCenterToWorldComponent>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entity = ecb.CreateEntity();
            ecb.AddComponent<ScreenCenterToWorldComponent>(entity);
            ecb.SetName(entity, "_ScreenCenterToWorldComponentSingleton");
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            _screenCenterToWorldSingleton = SystemAPI.GetSingletonEntity<ScreenCenterToWorldComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var screenComponent = SystemAPI.GetSingleton<ScreenComponent>();
            
            // return if screen size not changed
            if (Utils.IsScreenSizeChanged(screenComponent.WidthAndHeight.x, screenComponent.WidthAndHeight.y)) return;

            var camera = CameraSingleton.Instance.Camera;

            if (Physics.Raycast(
                    camera.ScreenPointToRay(new Vector3(screenComponent.ScreenCenter.x, screenComponent.ScreenCenter.y,
                        0f)), out var hit))
            {
                SystemAPI.SetComponent(_screenCenterToWorldSingleton, new ScreenCenterToWorldComponent
                {
                    ScreenCenterToWorld = new float3
                    (
                        Mathf.Round(hit.point.x),
                        Mathf.Round(hit.point.y),
                        Mathf.Round(hit.point.z)
                    )
                });
            }
        }
    }
}