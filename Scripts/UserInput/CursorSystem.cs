using Jrd.JCamera;
using Unity.Collections;
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
        private bool _lookingOnGroundPosition;
        private Entity _entity;
        private EntityManager _em;

        public void OnCreate(ref SystemState state)
        {
            _lookingOnGroundPosition = true; //TODO OFF/ON raycast mouse to ground
            _em = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            _entity = ecb.CreateEntity();
            ecb.AddComponent<CursorComponent>(_entity);
            ecb.SetName(_entity, CursorSystemEntityName);
            ecb.Playback(_em);
            ecb.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            var mousePosition = Input.mousePosition;

            if (float.IsPositiveInfinity(mousePosition.x) &&
                float.IsNegativeInfinity(mousePosition.y)) return;

            if (_lookingOnGroundPosition)
            {
                if (Physics.Raycast(CameraSingleton.Instance.Camera.ScreenPointToRay(mousePosition), out var hit))
                {
                    foreach (var cursor in SystemAPI.Query<RefRW<CursorComponent>>())
                    {
                        cursor.ValueRW.cursorPosition = hit.point;
                    }
                }
            }
        }
    }
}