using Jrd.DebSet;
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
            state.RequireForUpdate<BuildingPrefabComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (m_HUDInitialized)
                return;
            _em = state.EntityManager;
            var entity = SystemAPI.GetSingletonEntity<BuildingPrefabComponent>();
            _buildingPrefabComponent = _em.GetComponentData<BuildingPrefabComponent>(entity);

            BuildingPanelUI.BuildingCancel.clicked += () => H.T("BPU Cancel");
            BuildingPanelUI.Building1.clicked += GenBuild;
            BuildingPanelUI.Building2.clicked += () => H.T("BPU 2");
            m_HUDInitialized = true;
        }

        private void GenBuild()
        {
            // DebSetUI.DebSetText.text = "GenBuild";
            var entity = _em.Instantiate(_buildingPrefabComponent.Building1Prefab);
            _em.SetComponentData(entity, new LocalTransform
            {
                Position = new float3(3, 3, 3),
                Rotation = Quaternion.identity,
                Scale = 1
            });
        }
    }
}