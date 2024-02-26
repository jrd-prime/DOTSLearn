using Sources.Scripts.Game.Common;
using Sources.Scripts.Game.Features.Building.PlaceBuilding.Component;
using Sources.Scripts.Game.InitSystems;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UserInputAndCameraControl.CameraControl;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Sources.Scripts.Game.Features.Building.SelectBuilding

{
    public partial struct SelectBuildingSystem : ISystem
    {
        private EntityCommandBuffer _bsEcb;

        private Entity _tempTargetEntity;
        private Entity _tempFirstTargetEntity;

        private const uint TargetLayer = 1u << 31;

        private int _tempFingerId;
        private float _timeStart;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CameraData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            _tempFirstTargetEntity = Entity.Null;
        }

        // TODO KISS //LOOK
        public void OnUpdate(ref SystemState state)
        {
            if (Input.touchCount != 1) return;

            _bsEcb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var touch = Input.GetTouch(0);

            UnityEngine.Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                {
                    bool isHit = RaycastSystem.Raycast(ray, TargetLayer, out RaycastHit firstRaycastHit);

                    if (!isHit)
                    {
                        _tempFirstTargetEntity = Entity.Null;
                        return;
                    }

                    if (!IsMatchingTarget(firstRaycastHit.Entity, ref state)) return;

                    _timeStart = Time.time;
                    _tempFingerId = touch.fingerId;
                    _tempFirstTargetEntity = firstRaycastHit.Entity;
                    break;
                }
                case TouchPhase.Ended or TouchPhase.Canceled when _tempFingerId == touch.fingerId:
                {
                    if (Time.time - _timeStart > .3f) return;

                    bool isHit = RaycastSystem.Raycast(ray, TargetLayer, out RaycastHit secondRaycastHit);

                    if (!isHit) return;

                    if (!IsMatchingTarget(secondRaycastHit.Entity, ref state)) return;

                    if (_tempFirstTargetEntity != Entity.Null && _tempFirstTargetEntity == secondRaycastHit.Entity)
                    {
                        _bsEcb.AddComponent<SelectedBuildingTag>(_tempFirstTargetEntity);
                        _bsEcb.AddComponent<InitializeTag>(_tempFirstTargetEntity);
                        BuildingControlPanelUI.Instance.SetElementVisible(true);
                    }

                    _tempFingerId = -1;
                    break;
                }
            }
        }

        private bool IsMatchingTarget(Entity entity, ref SystemState _)
        {
            bool isBuilding = SystemAPI.HasComponent<BuildingTag>(entity);
            bool isTempBuilding = SystemAPI.HasComponent<TempBuildingTag>(entity);

            return !isTempBuilding || isBuilding;
        }
    }
}