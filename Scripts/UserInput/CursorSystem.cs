using Jrd.JCamera;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.UserInput
{
    /// <summary>
    /// Устанавливаем позицию курсора в мире
    /// </summary>
    public partial struct CursorSystem : ISystem
    {
        // TODO mobile logic
        private const string CursorSystemEntityName = "___ CursorSystemEntity";
        private bool _lookingOnGroundPosition;
        private Entity _entity;
        private EntityManager _em;

        public void OnCreate(ref SystemState state)
        {
            _lookingOnGroundPosition = true; //TODO OFF/ON raycast mouse to ground
            _em = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            _entity = ecb.CreateEntity();
            ecb.AddComponent<InputCursorData>(_entity);
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
                foreach (var cursor in SystemAPI.Query<RefRW<InputCursorData>>())
                {
                    if (Physics.Raycast(CameraMono.Instance.Camera.ScreenPointToRay(mousePosition), out var hit))
                    {
                        cursor.ValueRW.CursorWorldPosition = hit.point;
                    }

                    cursor.ValueRW.CursorScreenPosition = new float3(
                        math.clamp(Input.mousePosition.x, 0, UnityEngine.Screen.width),
                        math.clamp(Input.mousePosition.y, 0, UnityEngine.Screen.height),
                        0);
                    
                    cursor.ValueRW.CursorState = GetCursorState();
                }
            }
        }

        private CursorState GetCursorState()
        {
            if (Input.touchCount == 1)
            {
                //TODO add delay and fix
                return Input.touches[0].phase != TouchPhase.Ended || Input.touches[0].phase != TouchPhase.Canceled ? CursorState.ClickAndHold : CursorState.Click;

            }
            
            return CursorState.Default;
        }
    }
}