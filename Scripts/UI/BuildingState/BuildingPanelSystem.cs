using Jrd.GameStates;
using Unity.Entities;
using UnityEngine;

namespace Jrd.UI.BuildingState
{
    public partial struct BuildingPanelSystem : ISystem
    {
        private Entity _buildingPanelEntity;

        public void OnCreate(ref SystemState state)
        {
            _buildingPanelEntity = state.EntityManager.CreateEntity();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasComponent<ShowVisualElementTag>(_buildingPanelEntity))
            {
                Debug.Log("dont show");
            }else{
                Debug.Log("SHOW");}
        }
    }
}