using Jrd.GameplayBuildings;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Jrd.CameraControl;
using Jrd.UI.BuildingInfoPanel;
using Jrd.UI.BuildingState;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Jrd.GameStates.PlayState
{
    public partial struct SelectBuildingSystem : ISystem
    {
        private Entity _tempTargetEntity;
        private const uint TargetLayer = 1u << 31;
        private EntityCommandBuffer _bsEcb;
        private Entity _tempFirstTargetEntity;
        private int _tempFingerId;
        private float _timeStart;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CameraData>();
            state.RequireForUpdate<GameStateData>();
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
                    bool isHit = RaycastSystem.Raycast(ray, TargetLayer, out Entity firstEntity);

                    if (!isHit)
                    {
                        _tempFirstTargetEntity = Entity.Null;
                        return;
                    }

                    if (!IsMatchingTarget(firstEntity, ref state)) return;

                    _timeStart = Time.time;
                    _tempFingerId = touch.fingerId;
                    _tempFirstTargetEntity = firstEntity;
                    break;
                }
                case TouchPhase.Ended or TouchPhase.Canceled when _tempFingerId == touch.fingerId:
                {
                    if (Time.time - _timeStart > .3f) return;

                    bool isHit = RaycastSystem.Raycast(ray, TargetLayer, out Entity secondEntity);

                    if (!isHit) return;

                    if (!IsMatchingTarget(secondEntity, ref state)) return;

                    if (_tempFirstTargetEntity != Entity.Null && _tempFirstTargetEntity == secondEntity)
                    {
                        _bsEcb.AddComponent<SelectedBuildingTag>(_tempFirstTargetEntity);
                        _bsEcb.AddComponent<InitializeTag>(_tempFirstTargetEntity);
                        BuildingInfoPanelUIController.Instance.SetElementVisible(true);
                    }

                    _tempFingerId = -1;
                    break;
                }
            }
        }

        private bool IsMatchingTarget(Entity entity, ref SystemState state)
        {
            bool isBuilding = SystemAPI.HasComponent<BuildingTag>(entity);
            bool isTempBuilding = SystemAPI.HasComponent<TempBuildingTag>(entity);

            return !isTempBuilding || isBuilding;
        }
    }
}