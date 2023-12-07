using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.DebSet
{
    public partial struct DebSetSystem : ISystem
    {
        private Entity _entity;
        private EntityManager _em;
        private bool _isSubscribed;

        public void OnCreate(ref SystemState state)
        {
            _em = state.EntityManager;
            var archetype = _em.CreateArchetype(typeof(DebSetComponent));
            _entity = _em.CreateEntity(archetype);
            _em.SetName(_entity, "_DebSetEntity");
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_isSubscribed) return;

            DebSetUI.DebSetApplyButton.clicked += ApplyDebSettings;
            DebSetUI.DebSetClearLogButton.clicked += () => DebSetUI.DebSetText.text = "";

            _isSubscribed = true;
        }

        private void ApplyDebSettings()
        {
            H.T("ApplyDebSettings");
            _em.SetComponentData(_entity, new DebSetComponent
            {
                MouseRaycast = DebSetUI.IsMouseRaycast
            });
        }
    }
}