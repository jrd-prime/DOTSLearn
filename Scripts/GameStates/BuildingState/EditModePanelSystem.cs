using Jrd.GameStates;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Build.EditModePanel
{
    /// <summary>
    /// Показать/скрыть панель редактирования в режиме строительства
    /// </summary>
    [UpdateBefore(typeof(BuildSystem))]
    public partial struct EditModePanelSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;
        private Entity _gameStateEntity;

        public void OnCreate(ref SystemState state)
        {
            Debug.Log("EditModePanelSystem");
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            _em = state.EntityManager;

            var e = _ecb.CreateEntity();
            _ecb.SetName(e, "_Entity_EditModePanel");
            _ecb.AddComponent<EditModePanelComponent>(e);
            _ecb.Playback(_em);
            _ecb.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
        
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            // var a = SystemAPI.HasComponent<ShowVisualElementTag>(_gameStateEntity);
            // Debug.Log("has component " + a + " " + this);

            _ecb = new EntityCommandBuffer(Allocator.Temp);

            // show tag
            foreach (var editPanelShowTagQuery in SystemAPI.Query<EditModePanelComponent, VisualElementShowTag>()
                         .WithEntityAccess())
            {
                EditModeUI.ShowEditModePanel();
                _ecb.RemoveComponent<VisualElementShowTag>(editPanelShowTagQuery.Item3);
            }

            // hide tag
            foreach (var editPanelHideTagQuery in SystemAPI.Query<EditModePanelComponent, VisualElementHideTag>()
                         .WithEntityAccess())
            {
                EditModeUI.HideEditModePanel();
                _ecb.RemoveComponent<VisualElementHideTag>(editPanelHideTagQuery.Item3);
            }

            _ecb.Playback(_em);
            _ecb.Dispose();
        }
    }
}