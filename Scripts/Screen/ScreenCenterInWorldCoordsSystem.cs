using Jrd.JCamera;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.Screen
{
    /// <summary>
    /// Получаем мировые координаты точки с центра экрана
    /// </summary>
    [UpdateAfter(typeof(ScreenDataSystem))]
    public partial struct ScreenCenterInWorldCoordsSystem : ISystem
    {
        private Entity _screenCenterToWorldSingleton;
        private const int GroundLayerID = 1 << 3;
        private const float MaxDistance = 33f;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenComponent>();
            state.RequireForUpdate<ScreenCenterInWorldCoordsData>();

            _screenCenterToWorldSingleton = SystemAPI.GetSingletonEntity<ScreenCenterInWorldCoordsData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var screenComponent in SystemAPI.Query<RefRW<ScreenComponent>>())
            {
                var targetPoint = new Vector3(screenComponent.ValueRO.ScreenCenter.x,
                    screenComponent.ValueRO.ScreenCenter.y, 0f);

                if (!Physics.Raycast(
                        CameraMono.Instance.Camera.ScreenPointToRay(targetPoint),
                        out var hit,
                        MaxDistance,
                        GroundLayerID
                    )) return;

                // Debug.Log(new float3(
                //     Mathf.Round(hit.point.x),
                //     Mathf.Round(hit.point.y),
                //     Mathf.Round(hit.point.z)));

                if (Mathf.Round(hit.point.y) != 0)
                {
                    Debug.LogError("Ground Y != 0 or not set ground layer id");
                }

                SystemAPI.SetComponent(_screenCenterToWorldSingleton,
                    new ScreenCenterInWorldCoordsData
                    {
                        ScreenCenterToWorld = new float3(
                            Mathf.Round(hit.point.x),
                            Mathf.Round(hit.point.y),
                            Mathf.Round(hit.point.z))
                    });
            }
        }
    }
}