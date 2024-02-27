using Sources.Scripts.CommonComponents;
using Sources.Scripts.UserInputAndCameraControl.CameraControl;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Sources.Scripts.Screen.System
{
    /// <summary>
    /// Get world coordinates of the point
    /// from the center of the screen
    /// and set it to <see cref="ScreenCenterInWorldCoordsData"/> component
    /// </summary>
    [UpdateAfter(typeof(ScreenDataSystem))]
    public partial struct ScreenCenterInWorldCoordsSystem : ISystem
    {
        // TODO remove
        private const float MaxDistance = 33f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenData>();
            state.RequireForUpdate<ScreenCenterInWorldCoordsData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var screen in SystemAPI.Query<RefRW<ScreenData>>())
            {
                Entity centerToWorldEntity = SystemAPI.GetSingletonEntity<ScreenCenterInWorldCoordsData>();

                Vector3 targetPoint = new Vector3(screen.ValueRO.ScreenCenter.x, screen.ValueRO.ScreenCenter.y, 0f);

                if (!Physics.Raycast(
                        CameraMono.Instance.Camera.ScreenPointToRay(targetPoint),
                        out var hit,
                        MaxDistance,
                        (int)JLayers.GroundLayerID
                    )) return;
                
                Assert.IsTrue(
                    math.round(hit.point.y) == 0,
                    "Ground Y != 0 or not set ground layer id. And what??"
                );
                
                SystemAPI.SetComponent(centerToWorldEntity, new ScreenCenterInWorldCoordsData
                {
                    Value = new float3(
                        Mathf.Round(hit.point.x),
                        Mathf.Round(hit.point.y),
                        Mathf.Round(hit.point.z))
                });
            }
        }
    }
}