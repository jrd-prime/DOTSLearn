using Jrd.DebSet;
using Jrd.Grid.Points;
using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Jrd
{
    public partial class BuildingPanelSystem : SystemBase
    {
        private EntityManager _em;

        private bool _isBuildingPanelInitialized;
        private BuildingPrefabComponent _buildingPrefabComponent;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingPrefabComponent>();
        }

        protected override void OnUpdate()
        {
            if (_isBuildingPanelInitialized) return;
            
            _em = EntityManager;
            var entity = SystemAPI.GetSingletonEntity<BuildingPrefabComponent>();
            _buildingPrefabComponent = _em.GetComponentData<BuildingPrefabComponent>(entity);

            BuildingPanelUI.BuildingCancel.clicked += () => H.T("BPU Cancel");
            BuildingPanelUI.Building1.clicked += ChoosePrefabForBuild;
            BuildingPanelUI.Building2.clicked += () => H.T("BPU 2");

            _isBuildingPanelInitialized = true;
        }

        // выбираем то что будем строить и происходит цепочка событий которая размещает префаб в центре
        // экрана, записывает/читает данные по точкам
        // и в итоге показывает интрефейс для мува строения
        private void ChoosePrefabForBuild()
        {
            H.T("ChoosePrefabForBuild");
            Entity prefab = default; // TODO choose prefab

            // LOOK show edit mode UI
            ShowEditModeUI();
            
            // LOOK test prefab
            prefab = _buildingPrefabComponent.Building1Prefab;

            // LOOK подумать и вытащить в отдельную систему и подсистемы,
            // LOOK просто накидывать компоненты и тэги чтобы система подхватывала и в итоге было размещено
            PlacePrefabInCenterScreen(prefab);
        }

        private void ShowEditModeUI()
        {
            
        }

        private void PlacePrefabInCenterScreen(Entity prefab)
        {
            H.T("PlacePrefabInCenterScreen");
            var screenCenterPoint = Utils.GetScreenCenterPoint();

            var coords = GetPointCoordsInCenterScreen(screenCenterPoint);
            var point = GetPointByCoords(coords);

            if (point.self == Entity.Null) Debug.LogError("GetPointByCoords Entity NULL"); // ERROR

            PlacePrefab(prefab, point);
        }

        private void PlacePrefab(Entity prefab, PointComponent point)
        {
            H.T("PlacePrefab");
            var entity = _em.Instantiate(prefab);
            _em.SetComponentData(entity, new LocalTransform
            {
                Position = point.pointPosition,
                Rotation = Quaternion.identity,
                Scale = 1
            });
        }

        private PointComponent GetPointByCoords(float3 coords)
        {
            H.T("GetPointByCoords");
            // LOOK TODO dublicate, findpointundercursorsystem. FIX
            foreach (var point in SystemAPI.Query<RefRW<PointComponent>, RefRO<PointMainTagComponent>>())
            {
                var p = point.Item1.ValueRO;
                if (Equals(coords, p.pointPosition))
                {
                    H.T("Entity" + p.self);
                    return p;
                    // temp scale +
                    _em.SetComponentData(p.self, new LocalTransform
                    {
                        Position = coords,
                        Scale = .2f,
                        Rotation = Quaternion.identity
                    });
                }
            }

            return new PointComponent { self = Entity.Null }; // LOOK FIX
        }

        private float3 GetPointCoordsInCenterScreen(Vector3 screenCenterPoint)
        {
            H.T("GetPointCoordsInCenterScreen");
            if (Physics.Raycast(CameraSingleton.Instance.Camera.ScreenPointToRay(screenCenterPoint), out var hit))
            {
                return new float3
                (
                    Mathf.Floor(hit.point.x),
                    0,
                    Mathf.Floor(hit.point.z)
                );
            }

            return float3.zero;
        }
    }
}