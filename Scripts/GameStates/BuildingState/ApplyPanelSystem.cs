using Jrd.Build;
using Jrd.Build.old;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates.BuildingState
{
    /// <summary>
    /// Показать/скрыть панель редактирования в режиме строительства
    /// </summary>
    [UpdateAfter(typeof(BSBuildingStateSystem))]
    public partial struct ApplyPanelSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;
        private Entity _gameStateEntity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildPrefabsComponent>();
        }

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
                ApplyPanelUI.ShowApplyPanel();
                ecb.RemoveComponent<BSApplyPanelShowTag>(_gameStateEntity);
            }

            foreach (var q in SystemAPI.Query<BSApplyPanelComponent, BSApplyPanelHideTag>())
            {
                // Debug.Log("hide " + this);
                ApplyPanelUI.HideApplyPanel();
                ecb.RemoveComponent<BSApplyPanelHideTag>(_gameStateEntity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}