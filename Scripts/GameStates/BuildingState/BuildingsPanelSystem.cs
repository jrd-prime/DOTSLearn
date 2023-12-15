using Jrd.Build.old;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
        private Entity _gameStateEntity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildPrefabsComponent>();
            _em = state.EntityManager;
        }

        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            if (!SystemAPI.HasComponent<BSBuildingsPanelComponent>(_gameStateEntity)) return;


            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var unused in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelShowTag>())
            {
                // Debug.Log("show");
                BuildingPanelUI.InstantiateButtons(SystemAPI.GetComponent<BuildingStateComponent>(_gameStateEntity)
                    .PrefabsCount);
                BuildingPanelUI.ShowApplyPanel();
                ecb.RemoveComponent<BSBuildingsPanelShowTag>(_gameStateEntity);
            }

            foreach (var unused in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelHideTag>())
            {
                // Debug.Log("hide");
                BuildingPanelUI.HideApplyPanel();
                ecb.RemoveComponent<BSBuildingsPanelHideTag>(_gameStateEntity);
            }

            ecb.Playback(_em);
            ecb.Dispose();
        }
    }
}