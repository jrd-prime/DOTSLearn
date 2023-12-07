using Jrd.DebSet;
using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Jrd
{
    public partial struct BuildingPanelSystem : ISystem
    {
        private EntityManager _em;

        private bool _isBuildingPanelInitialized;
        private BuildingPrefabComponent _buildingPrefabComponent;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingPrefabComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_isBuildingPanelInitialized) return;

            _em = state.EntityManager;
            var entity = SystemAPI.GetSingletonEntity<BuildingPrefabComponent>();
            _buildingPrefabComponent = _em.GetComponentData<BuildingPrefabComponent>(entity);

            BuildingPanelUI.BuildingCancel.clicked += () => H.T("BPU Cancel");
            BuildingPanelUI.Building1.clicked += ChoosePrefabForBuild;
            BuildingPanelUI.Building2.clicked += () => H.T("BPU 2");

            _isBuildingPanelInitialized = true;
        }

        private void ChoosePrefabForBuild()
        {
            H.T("ChoosePrefabForBuild");
            Entity prefab = default;

            PlacePrefabInCenterScreen(prefab);
        }

        private void PlacePrefabInCenterScreen(Entity prefab)
        {
            H.T("PlacePrefabInCenterScreen");
            var screenCenterPoint = Utils.GetScreenCenterPoint();
        }

        private void GenBuild()
        {
            H.T("GenBuild");


            var centerPosition = float3.zero;
            // if (Physics.Raycast(CameraSingleton.Instance.Camera.ScreenPointToRay(screenCenter), out var hit))
            // {
            //     centerPosition = new Vector3
            //     (
            //         Mathf.Floor(hit.point.x),
            //         0,
            //         Mathf.Floor(hit.point.z)
            //     );
            //     H.T(centerPosition.ToString());
            // }


            var entity = _em.Instantiate(_buildingPrefabComponent.Building1Prefab);
            _em.SetComponentData(entity, new LocalTransform
            {
                Position = centerPosition,
                Rotation = Quaternion.identity,
                Scale = 1
            });
        }
    }
}