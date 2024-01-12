using System;
using Jrd.GameStates.MainGameState;
using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Jrd
{
    /// <summary>
    /// v0.0.1
    /// Add the SelectedTag to the clicked temp building
    /// </summary>
    // make common?
    [BurstCompile]
    public partial struct TempBuildingSelectionSystem : ISystem // TODO +job
    {
        private const float RayDistance = 200f;
        private Entity _tempTargetEntity;
        private bool _isSelectTagAdded;
        private const uint TargetLayer = 1u << 31;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraComponent>();
            state.RequireForUpdate<GameStateData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            _isSelectTagAdded = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.GetSingleton<GameStateData>().CurrentGameState != GameState.BuildingState)
                return; //TODO refact

            if (Input.touchCount != 1) return; //TODO more than 1 touch???

            var touch = Input.GetTouch(0);

            var b = SystemAPI.GetSingletonEntity<CameraComponent>();
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);
                    if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity targetEntity)) break;

                    // if (SystemAPI.HasComponent<TempBuildingTag>(targetEntity)) Debug.Log("it's temp building!"); 

                    _tempTargetEntity = targetEntity;

                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (_tempTargetEntity != Entity.Null)
                    {
                        Debug.Log("Temp target exist. Do stuff.");
                        
                        //TODO bad idea
                        state.EntityManager.RemoveComponent<MoveDirectionData>(b);
                        if (!_isSelectTagAdded)
                        {
                            state.EntityManager.AddComponent<SelectedTag>(_tempTargetEntity); //TODO ecb

                            _isSelectTagAdded = true;
                        }
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Debug.Log("Touch ended or cancelled.");

                    //TODO bad idea
                    state.EntityManager.AddComponent<MoveDirectionData>(b);

                    if (_isSelectTagAdded)
                    {
                        state.EntityManager.RemoveComponent<SelectedTag>(_tempTargetEntity); //TODO ecb
                        _isSelectTagAdded = false;
                    }

                    _tempTargetEntity = Entity.Null;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [BurstCompile]
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

    // [MaterialProperty("_jrd")]
    // public struct MyOwnColor : IComponentData
    // {
    //     public float4 Value;
    // }
}