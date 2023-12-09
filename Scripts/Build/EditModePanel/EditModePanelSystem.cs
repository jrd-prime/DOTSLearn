using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
                        EditModeUI.ShowEditModePanel();
                        a.IsVisible = true;
                        break;
                    case false:
                        EditModeUI.HideEditModePanel();
                        a.IsVisible = false;
                        break;
                }
            }
        }
    }
}