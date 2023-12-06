using Jrd.Grid.GridLayout;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;

namespace Jrd.DebSet
{
    public partial struct DebSetSystem : ISystem
    {
        private Entity _entity;
        private EntityManager _em;
        private bool _isSetted;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridComponent>();
            _em = state.EntityManager;
            _entity = _em.CreateEntity(typeof(DebSetComponent));
            _em.SetName(_entity, "_DebSetEntity");
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_isSetted) return;
            Profiler.BeginSample("test");
            DebSetUI.DebSetApplyButton.clicked += Sett;
            Profiler.EndSample();
            _isSetted = true;
        }

        private void Sett()
        {
            _em.SetComponentData(_entity, new DebSetComponent
            {
                mouseRaycast = DebSetUI.IsMouseRaycast
            });
        }
    }
}