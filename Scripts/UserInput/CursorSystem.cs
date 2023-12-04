using Jrd.JCamera;
using Unity.Entities;
using UnityEngine;

namespace Jrd.UserInput
{
    /// <summary>
    /// Устанавливаем позицию курсора в мире
    /// </summary>
    public partial struct CursorSystem : ISystem
    {
        // TODO mobile logic
        private const string CursorSystemEntityName = "_CursorSystemEntity";

        private Entity _entity;
        private EntityManager _em;

        public void OnCreate(ref SystemState state)
        {
            _em = state.EntityManager;
            _entity = _em.CreateEntity();
            _em.AddComponent<CursorComponent>(_entity);
            _em.SetName(_entity, CursorSystemEntityName);
        }

        public void OnUpdate(ref SystemState state)
        {
            var camera = CameraSingleton.Instance.Camera;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                foreach (var cursorComponent in SystemAPI.Query<RefRW<CursorComponent>>())
                {
                    cursorComponent.ValueRW.cursorPosition = hit.point;
                }
            }
        }
    }
}