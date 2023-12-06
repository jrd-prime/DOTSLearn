using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd
{
    public partial struct BuildingPanelSystem : ISystem
    {
        private EntityManager _em;

        private bool m_HUDInitialized;
        private BuildingPrefabComponent _buildingPrefabComponent;

        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            if (m_HUDInitialized)
                return;
            _em = state.EntityManager;
            SystemAPI.TryGetSingletonEntity<BuildingPrefabComponent>(out var entity);
            _buildingPrefabComponent = _em.GetComponentData<BuildingPrefabComponent>(entity);

            BuildingPanelUI.BuildingCancel.clicked += () => Debug.Log("BPU Cancel");

            BuildingPanelUI.Building1.clicked += GenBuild;
            BuildingPanelUI.Building2.clicked += () => Debug.Log("BPU 2");
            m_HUDInitialized = true;
        }

        private void GenBuild()
        {
            Debug.Log("BPU 1");
            var _entity = _em.Instantiate(_buildingPrefabComponent.Building1Prefab);
            _em.SetComponentData(_entity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = Quaternion.identity,
                Scale = 1
            });
            
        }
    }
}