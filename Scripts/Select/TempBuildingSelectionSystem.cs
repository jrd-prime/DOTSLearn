using System;
using Jrd.CameraControl;
using Jrd.Gameplay.Building.TempBuilding;
using Jrd.Gameplay.Building.TempBuilding.Component;
using Jrd.GameStates.MainGameState;
using Jrd.UserInput.Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Ray = UnityEngine.Ray;

namespace Jrd.Select
{
    /// <summary>
    /// Add the SelectedTag to the clicked temp building
    /// </summary>
    public partial struct TempBuildingSelectionSystem : ISystem // TODO +job
    {
        private const float RayDistance = 200f;
        private Entity _tempTargetEntity;
        private bool _isSelectTagAdded;
        private const uint TargetLayer = 1u << 31;
        private EntityCommandBuffer _bsEcb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<TempBuildingTag>();
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

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);

                    bool isHit = RaycastSystem.Raycast(ray, TargetLayer, out Entity targetEntity);

                    if (!isHit) break;

                    // if (SystemAPI.HasComponent<TempBuildingTag>(targetEntity)) Debug.Log("it's temp building!"); 

                    _tempTargetEntity = targetEntity;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (_tempTargetEntity != Entity.Null)
                    {
                        Debug.Log($"Temp target exist. Do stuff. {_tempTargetEntity}");

                        //TODO bad idea
                        _bsEcb.RemoveComponent<MoveDirectionData>(cameraEntity);
                        if (!_isSelectTagAdded)
                        {
                            _bsEcb.AddComponent<SelectedTag>(_tempTargetEntity);
                            _isSelectTagAdded = true;
                        }
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Debug.Log("Touch ended or cancelled.");

                    //TODO bad idea
                    _bsEcb.AddComponent<MoveDirectionData>(cameraEntity);

                    if (_isSelectTagAdded)
                    {
                        _bsEcb.RemoveComponent<SelectedTag>(_tempTargetEntity);
                        _isSelectTagAdded = false;
                    }

                    _tempTargetEntity = Entity.Null;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}