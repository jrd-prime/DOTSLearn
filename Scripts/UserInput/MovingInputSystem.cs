using Jrd.JUtils;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.UserInput
{
    /// <summary>
    /// Устанавливает вектор движения simple
    /// </summary>
    public partial struct MovingInputSystem : ISystem
    {
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";

        private const float YAxis = 0f;
        private int _fingerId;
        private const float MinDelta = -1;
        private const float MaxDelta = 1;

        public void OnUpdate(ref SystemState state)
        {
#if UNITY_EDITOR
            SetDirection(Input.GetAxis(Horizontal), Input.GetAxis(Vertical), ref state);
#endif
            // LOOK зум. баг. когда отпускаешь 23 пальца, то двигается без контроля мувмент
            // #if UNITY_ANDROID
            // if (Input.touchCount > 0)
            // {
            if (Input.touchCount == 1)
            {
                if (!Utils.IsPointerOverUIObject())
                {
                    var touch = Input.GetTouch(0);
                    _fingerId = touch.fingerId;

                    switch (touch.phase)
                    {
                        case TouchPhase.Moved when touch.fingerId == _fingerId:
                        {
                            // Debug.Log("Moved");
                            var posDelta = touch.deltaPosition * SystemAPI.Time.DeltaTime * -1;
                            SetDirection(Clamp(posDelta.x), Clamp(posDelta.y), ref state);
                            break;
                        }
                        case TouchPhase.Ended when touch.fingerId == _fingerId:
                            // Debug.Log("Ended");
                            SetDirection(0, 0, ref state);
                            break;
                        case TouchPhase.Canceled when touch.fingerId == _fingerId:
                            // Debug.Log("Canceled");
                            SetDirection(0, 0, ref state);
                            break;
                    }
                }
                else
                {
                    SetDirection(0, 0, ref state);
                }
            }
            // }
// #endif
        }

        private void SetDirection(float x, float z, ref SystemState state)
        {
            foreach (var query in SystemAPI.Query<RefRW<MoveDirectionData>>())
            {
                query.ValueRW.Direction = (x != 0 || z != 0)
                    ? new float3(x, YAxis, z)
                    : Vector3.zero;
            }
        }

        private float Clamp(float position)
        {
            return Mathf.Clamp(position, MinDelta, MaxDelta);
        }
    }
}