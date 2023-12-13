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
    public partial struct BuildingsPanelSystem : ISystem
    {
        private Entity _gameStateEntity;

        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;
            
            if (SystemAPI.HasComponent<BuildingsPanelData>(_gameStateEntity)) return;
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var q in SystemAPI.Query<BuildingsPanelData, ShowVisualElementTag>())
            {
                // Debug.Log("show");
                BuildingPanelUI.SetRootDisplay(DisplayStyle.Flex);
                ecb.RemoveComponent<ShowVisualElementTag>(_gameStateEntity);
            }

            foreach (var q in SystemAPI.Query<BuildingsPanelData, HideVisualElementTag>())
            {
                // Debug.Log("hide");
                BuildingPanelUI.SetRootDisplay(DisplayStyle.None);
                ecb.RemoveComponent<HideVisualElementTag>(_gameStateEntity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}