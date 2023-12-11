using Jrd.JCamera;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.Build.Screen
{
    /// <summary>
    /// Получаем мировые координаты точки с центра экрана
    /// </summary>
     public partial struct ScreenCenterInWorldCoordsSystem : ISystem
    {
        private Entity _screenCenterToWorldSingleton;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenComponent>();
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entity = ecb.CreateEntity();
            ecb.AddComponent<ScreenCenterInWorldCoordsComponent>(entity);
            ecb.SetName(entity, "_ScreenCenterToWorldComponentSingleton");
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            
            _screenCenterToWorldSingleton = SystemAPI.GetSingletonEntity<ScreenCenterInWorldCoordsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            SetScreenCenterToWorldCoords(SystemAPI.GetSingleton<ScreenComponent>(), ref state);
        }

        private void SetScreenCenterToWorldCoords(ScreenComponent screenComponent, ref SystemState state)
        {
            if (!Physics.Raycast(
                    CameraSingleton.Instance.Camera.ScreenPointToRay(new Vector3(screenComponent.ScreenCenter.x,
                        screenComponent.ScreenCenter.y, 0f)), out var hit)) return;

            SystemAPI.SetComponent(_screenCenterToWorldSingleton, new ScreenCenterInWorldCoordsComponent
            {
                ScreenCenterToWorld = new float3(
                    Mathf.Round(hit.point.x),
                    Mathf.Round(hit.point.y),
                    Mathf.Round(hit.point.z))
            });
        }
    }
}