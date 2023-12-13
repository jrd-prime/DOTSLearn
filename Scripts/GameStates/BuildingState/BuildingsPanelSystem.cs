using Jrd.GameStates.BuildingState.Tag;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    /// <summary>
    /// Панель с выбором построек
    /// - включается, когда присутствует BuildingState + BuildingsPanel
    ///
    /// * инициализирует и показывает/скрывает панель построек
    /// </summary>
     [UpdateAfter(typeof(BSBuildingStateSystem))]
    public partial struct BuildingsPanelSystem : ISystem
    {
        private Entity _gameStateEntity;

        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;
            
            if (!SystemAPI.HasComponent<BSBuildingsPanelComponent>(_gameStateEntity)) return;
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var q in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelShowTag>())
            {
                // Debug.Log("show");
                BuildingPanelUI.ShowEditModePanel();
                ecb.RemoveComponent<BSBuildingsPanelShowTag>(_gameStateEntity);
            }

            foreach (var q in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelHideTag>())
            {
                // Debug.Log("hide");
                BuildingPanelUI.HideEditModePanel();
                ecb.RemoveComponent<BSBuildingsPanelHideTag>(_gameStateEntity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}