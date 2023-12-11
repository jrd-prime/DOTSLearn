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
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial struct EditModePanelSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;

        public void OnCreate(ref SystemState state)
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp, PlaybackPolicy.SinglePlayback);
            Debug.Log("EditModePanelSystem");

            _em = state.EntityManager;

            var e = _ecb.CreateEntity();
            _ecb.SetName(e, "_Entity_EditModePanel");
            _ecb.AddComponent<EditModePanelComponent>(e);
            _ecb.Playback(_em);
            _ecb.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
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