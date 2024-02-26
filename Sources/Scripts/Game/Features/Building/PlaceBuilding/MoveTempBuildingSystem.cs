using GamePlay.Features.Building.PlaceBuilding.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UserInputAndCameraControl.CameraControl;
using UserInputAndCameraControl.UserInput.Components;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace GamePlay.Features.Building.PlaceBuilding
{
    public partial struct MoveTempBuildingSystem : ISystem
    {
        private const uint GroundLayer = 1u << 3;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<CameraData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (Input.touchCount == 0) return;

            foreach (var (move, transform) in SystemAPI
                         .Query<RefRO<MoveDirectionData>, RefRW<LocalTransform>>()
                         .WithAll<TempBuildingTag, SelectedBuildingTag, SelectableBuildingTag>())
            {
                Touch touch = Input.GetTouch(0);

                Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);

                if (!Common.RaycastSystem.Raycast(ray, GroundLayer, out RaycastHit hitPosition)) return;

                float3 position = new(math.round(hitPosition.Position.x), 0, math.round(hitPosition.Position.z));

                transform.ValueRW.Position = position;
            }
        }
    }
}