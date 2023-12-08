using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.Build.EditModePanel
{
    public partial struct EditModePanelSystem : ISystem
    {
        private EntityCommandBuffer ecb;
        private EntityManager _em;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EditModePanelComponent>();
            ecb = new EntityCommandBuffer(Allocator.Temp);
            _em = state.EntityManager;
        }

        public void OnUpdate(ref SystemState state)
        {
            // state.Enabled = false;

            foreach (var b in SystemAPI.Query<RefRW<EditModePanelComponent>>())
            {
                var a = b.ValueRW;
                Debug.Log(a.ShowPanel);
                switch (a.ShowPanel)
                {
                    case true:
                        ShowEditModePanel();
                        a.IsVisible = true;
                        break;
                    case false:
                        HideEditModePanel();
                        a.IsVisible = false;
                        break;
                }
            }
        }

        private void ShowEditModePanel()
        {
            var root = EditModeUI.EditModeRoot;
            if (root.style.display == DisplayStyle.Flex) return;

            root.style.display = DisplayStyle.Flex;
            var showEditMenuPanelAnimation = root.experimental.animation.Start(
                new StyleValues { bottom = -100f },
                new StyleValues { bottom = 0f }, 1000).Ease(Easing.OutElastic).KeepAlive();
        }

        private void HideEditModePanel()
        {
            var root = EditModeUI.EditModeRoot;
            if (root.style.display == DisplayStyle.None) return;

            var hideEditModePanelAnimation = EditModeUI.EditModePanel.experimental.animation.Start(
                new StyleValues { bottom = 0f },
                new StyleValues { bottom = -100f }, 500).Ease(Easing.InSine).KeepAlive();
            hideEditModePanelAnimation.onAnimationCompleted =
                () => root.style.display = DisplayStyle.None;
        }
    }
}