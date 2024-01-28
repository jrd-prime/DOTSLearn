using System;
using Jrd.GameplayBuildings;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Ray = Unity.Physics.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Jrd.GameStates.PlayState
{
    public partial struct SelectBuildingSystem : ISystem
    {
        private const float RayDistance = 200f;
        private Entity _tempTargetEntity;
        private bool _isSelectTagAdded;
        private const uint TargetLayer = 1u << 31;
        private EntityCommandBuffer _bsEcb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            // state.RequireForUpdate<TempBuildingTag>();

            state.RequireForUpdate<CameraData>();
            state.RequireForUpdate<GameStateData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();

            _isSelectTagAdded = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (Input.touchCount != 1) return; //TODO more than 1 touch???

            _bsEcb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            Touch touch = Input.GetTouch(0);
            Entity cameraEntity = SystemAPI.GetSingletonEntity<CameraData>();

            UnityEngine.Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);
            if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity targetEntity)) return;

            bool isTempBuilding = SystemAPI.HasComponent<TempBuildingTag>(targetEntity);
            bool isBuilding = SystemAPI.HasComponent<BuildingTag>(targetEntity);

            if (isTempBuilding || !isBuilding)
            {
                Debug.Log("ITS TEMP OR NOT BUILDING. Return");
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    Debug.Log("click in building " + targetEntity);
                    _tempTargetEntity = targetEntity;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Debug.Log("RETURN");
                    return;
                default:
                    return;
            }
        }

        public bool Raycast(float3 from, float3 to, out Entity entity)
        {
            var input = new RaycastInput
            {
                Start = from,
                End = to,
                Filter = new CollisionFilter
                {
                    BelongsTo = TargetLayer,
                    CollidesWith = TargetLayer,
                    GroupIndex = 0
                }
            };

            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            if (collisionWorld.CastRay(input, out RaycastHit hit))
            {
                entity = hit.Entity;
                return true;
            }

            entity = Entity.Null;
            return false;
        }
    }
}