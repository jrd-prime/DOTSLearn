using Jrd.CameraControl;
using Jrd.UserInput.Components;
using Jrd.Utils.Const;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.UserInput.Systems
{
    /// <summary>
    /// Устанавливаем позицию курсора в мире
    /// </summary>
    // [BurstCompile]
    public partial struct CursorSystem : ISystem
    {
        // TODO mobile logic
        private bool _lookingOnGroundPosition;
        private Entity _entity;
        private EntityManager _em;

        // [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _lookingOnGroundPosition = true; //TODO OFF/ON raycast mouse to ground
            _em = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp); //TODO ecb refact
            _entity = ecb.CreateEntity();
            ecb.AddComponent<InputCursorData>(_entity);
            ecb.SetName(_entity, ENames.InputCursorDataEntityName);
            ecb.Playback(_em);
            ecb.Dispose();
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var mousePosition = new float3(
                math.clamp(Input.mousePosition.x, 0, UnityEngine.Screen.width),
                math.clamp(Input.mousePosition.y, 0, UnityEngine.Screen.height),
                0);

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

        // [BurstCompile]
        private CursorState GetCursorState()
        {
            if (Input.touchCount == 1)
            {
                //TODO add delay and fix
                return Input.touches[0].phase != TouchPhase.Ended || Input.touches[0].phase != TouchPhase.Canceled
                    ? CursorState.ClickAndHold
                    : CursorState.Click;
            }

            return CursorState.Default;
        }
    }
}