using Jrd.Build;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI.EditModeUI;
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
        private EntityManager _em;
        private Entity _buildPrefabComponentEntity;

        private Entity _gameStateEntity;
        private bool _isSubscribed;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildPrefabComponent>();
            _em = state.EntityManager;
        }

        public void OnUpdate(ref SystemState state)
        {
            _buildPrefabComponentEntity = SystemAPI.GetSingletonEntity<BuildPrefabComponent>();
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            if (!SystemAPI.HasComponent<BSBuildingsPanelComponent>(_gameStateEntity)) return;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var unused in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelShowTag>())
            {
                // Debug.Log("show");
                BuildingPanelUI.ShowEditModePanel();
                ecb.RemoveComponent<BSBuildingsPanelShowTag>(_gameStateEntity);
            }

            foreach (var unused in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelHideTag>())
            {
                // Debug.Log("hide");
                BuildingPanelUI.HideEditModePanel();
                ecb.RemoveComponent<BSBuildingsPanelHideTag>(_gameStateEntity);
            }

            ecb.Playback(_em);
            ecb.Dispose();

            if (_isSubscribed) return;
            BuildingPanelUI.OnBuildingButtonClicked += ChooseBuilding;
            // EditModeUI.EditModeCancelButton.clicked += ExitFromEditMode;
            _isSubscribed = true;
        }


        private void ChooseBuilding(Button button)
        {
            Debug.Log("button = " + button);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var a = _em.GetBuffer<PrefabBufferElements>(_buildPrefabComponentEntity);
            
            
            ecb.Playback(_em);
            ecb.Dispose();
        }

        public void OnDestroy(ref SystemState state)
        {
            BuildingPanelUI.OnBuildingButtonClicked -= ChooseBuilding;
        }
    }
}