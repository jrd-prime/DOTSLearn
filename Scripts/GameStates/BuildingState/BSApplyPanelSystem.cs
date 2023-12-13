using Jrd.GameStates;
using Jrd.GameStates.BuildingState;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Build.EditModePanel
{
    /// <summary>
    /// Показать/скрыть панель редактирования в режиме строительства
    /// </summary>
    [UpdateAfter(typeof(BSBuildingStateSystem))]
    public partial struct BSApplyPanelSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;
        private Entity _gameStateEntity;
        
        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            if (!SystemAPI.HasComponent<BSApplyPanelComponent>(_gameStateEntity)) return;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var q in SystemAPI.Query<BSApplyPanelComponent, BSApplyPanelShowTag>())
            {
                // Debug.Log("show " + this);
                EditModeUI.ShowEditModePanel();
                ecb.RemoveComponent<BSApplyPanelShowTag>(_gameStateEntity);
            }

            foreach (var q in SystemAPI.Query<BSApplyPanelComponent, BSApplyPanelHideTag>())
            {
                // Debug.Log("hide " + this);
                EditModeUI.HideEditModePanel();
                ecb.RemoveComponent<BSApplyPanelHideTag>(_gameStateEntity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}