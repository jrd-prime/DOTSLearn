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
        private Entity tempFirstTargetEntity;
        private int tempFingerId;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<CameraData>();
            state.RequireForUpdate<GameStateData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            _isSelectTagAdded = false;
            tempFirstTargetEntity = Entity.Null;
        }

        public void OnUpdate(ref SystemState state)
        {
            // LOOK TODO refactor with wait time and deltapos

            if (Input.touchCount != 1) return; //TODO more than 1 touch???

            _bsEcb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var touch = Input.GetTouch(0);

            UnityEngine.Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity firstEntity))
                {
                    tempFirstTargetEntity = Entity.Null;
                    return;
                }

                tempFingerId = touch.fingerId;
                tempFirstTargetEntity = firstEntity;
                bool isTempBuilding = SystemAPI.HasComponent<TempBuildingTag>(firstEntity);
                bool isBuilding = SystemAPI.HasComponent<BuildingTag>(firstEntity);

                if (isTempBuilding || !isBuilding)
                {
                    Debug.Log("ITS TEMP OR NOT BUILDING. Return");
                    return;
                }
            }

            int fingerId = touch.fingerId;
            if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled && tempFingerId == fingerId)
            {
                if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity secondEntity)) return;

                bool isTempBuilding = SystemAPI.HasComponent<TempBuildingTag>(secondEntity);
                bool isBuilding = SystemAPI.HasComponent<BuildingTag>(secondEntity);


                if (isTempBuilding || !isBuilding)
                {
                    Debug.Log("ITS TEMP OR NOT BUILDING. Return");
                    return;
                }


                if (tempFirstTargetEntity != Entity.Null && tempFirstTargetEntity == secondEntity)
                {
                    Debug.LogWarning(" DO STUFF !");
                    ConfirmationPanelMono.Instance.Show();
                }

                tempFingerId = -1;
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