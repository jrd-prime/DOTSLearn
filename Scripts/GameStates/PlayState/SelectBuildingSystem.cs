using Jrd.GameplayBuildings;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Jrd.JCamera;
using Jrd.UI.BuildingState;
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
        private Entity _tempFirstTargetEntity;
        private int _tempFingerId;
        private float _timeStart;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CameraData>();
            state.RequireForUpdate<GameStateData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            _isSelectTagAdded = false;
            _tempFirstTargetEntity = Entity.Null;
        }

        public void OnUpdate(ref SystemState state)
        {
            // TODO KISS //LOOK

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
                    if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity firstEntity))
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

                    if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity secondEntity)) return;

                    if (!IsMatchingTarget(secondEntity, ref state)) return;

                    if (_tempFirstTargetEntity != Entity.Null && _tempFirstTargetEntity == secondEntity)
                    {
                        Debug.LogWarning(" DO STUFF !");
                        ConfirmationPanelMono.Instance.Show();
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