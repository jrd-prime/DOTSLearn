using Sources.Scripts.CommonComponents;
using Sources.Scripts.Game.Common;
using Sources.Scripts.UserInputAndCameraControl.CameraControl;
using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct MoveTempBuildingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CameraData>();
        }

        public unsafe void OnUpdate(ref SystemState state)
        {
            if (Input.touchCount == 0) return;

            foreach (var (move, transform) in SystemAPI
                         .Query<RefRO<MoveDirectionData>, RefRW<LocalTransform>>()
                         .WithAll<TempBuildingTag, SelectedBuildingTag, SelectableBuildingTag>())
            {
                Touch touch = Input.GetTouch(0);

                Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);

                if (!RaycastSystem.Raycast(ray, JLayers.GroundLayerID, out RaycastHit hitPosition)) return;

                fixed (float3* a = &transform.ValueRW.Position)
                {
                    a->x = math.round(hitPosition.Position.x);
                    a->y = 0;
                    a->z = math.round(hitPosition.Position.z);
                }
            }
        }
    }
}