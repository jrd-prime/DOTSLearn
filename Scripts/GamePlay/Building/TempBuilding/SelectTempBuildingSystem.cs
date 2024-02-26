using System;
using GamePlay.Building.TempBuilding.Component;
using GamePlay.GameStates.MainGameState;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UserInputAndCameraControl.CameraControl;
using UserInputAndCameraControl.UserInput.Components;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace GamePlay.Building.TempBuilding
{
    /// <summary>
    /// Add the SelectedTag to the clicked temp building
    /// </summary>
    public partial struct SelectTempBuildingSystem : ISystem // TODO +job
    {
        private EntityCommandBuffer _bsEcb;
        private Entity _tempTargetEntity;

        private const float RayDistance = 200f;
        private const uint TargetLayer = 1u << 31;

        private bool _isSelectTagAdded;

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

                    bool isHit = RaycastSystem.Raycast(ray, TargetLayer, out RaycastHit raycastHit);

                    if (!isHit) break;

                    // if (SystemAPI.HasComponent<TempBuildingTag>(targetEntity)) Debug.Log("it's temp building!"); 

                    _tempTargetEntity = raycastHit.Entity;
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
                            _bsEcb.AddComponent<SelectedBuildingTag>(_tempTargetEntity);
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
                        _bsEcb.RemoveComponent<SelectedBuildingTag>(_tempTargetEntity);
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