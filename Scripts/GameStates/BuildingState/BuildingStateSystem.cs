using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates.BuildingState
{
    public partial struct BuildingStateSystem : ISystem
    {
        private EntityManager _em;

        private Entity _buildingPanel;
        private Entity _confirmationPanel;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _em = state.EntityManager;
            foreach (var tag in SystemAPI.Query<InitializeTag>().WithAll<BuildingStateComponent>())
            {
                Debug.Log("BuildingStateComponent + InitializeTag");
            }

            // create panel entity
            if (_buildingPanel == Entity.Null)
                _buildingPanel = GetCustomEntity<BuildingPanelComponent>(BSConst.BuildingPanelEntityName);
            // create apply panel entity
            if (_confirmationPanel == Entity.Null)
                _confirmationPanel = GetCustomEntity<ConfirmationPanelComponent>(BSConst.ConfirmationPanelEntityName);
        }

        private Entity GetCustomEntity<T>(FixedString64Bytes entityName) where T : unmanaged, IComponentData
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entity = ecb.CreateEntity();
            var nameWithPrefix = BSConst.Prefix + " " + entityName;
            ecb.AddComponent<T>(entity);
            ecb.SetName(entity, nameWithPrefix);
            ecb.Playback(_em);
            ecb.Dispose();
            return entity;
        }
    }
}