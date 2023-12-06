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

        private float _xAxis;
        private const float YAxis = 0f;
        private float _zAxis;
        private int _fingerId;

        public void OnUpdate(ref SystemState state)
        {
            // var horizontalAxis = Input.GetAxisRaw(Horizontal);
            // var verticalAxis = Input.GetAxisRaw(Vertical);
#if UNITY_EDITOR
            _xAxis = Input.GetAxis(Horizontal);
            _zAxis = Input.GetAxis(Vertical);
#endif

#if UNITY_ANDROID
            if (Input.touchCount > 0)
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
                            _xAxis = Mathf.Clamp(posDelta.x, -1, 1);
                            _zAxis = Mathf.Clamp(posDelta.y, -1, 1);
                            break;
                        }
                        case TouchPhase.Ended when touch.fingerId == _fingerId:
                            // Debug.Log("Ended");
                            _xAxis = 0;
                            _zAxis = 0;
                            break;
                        case TouchPhase.Canceled when touch.fingerId == _fingerId:
                            // Debug.Log("Ended");
                            _xAxis = 0;
                            _zAxis = 0;
                            break;
                    }
                }
                else
                {
                    _xAxis = 0;
                    _zAxis = 0;
                }
            }
#endif

            foreach (var query in SystemAPI.Query<RefRW<MovingEventComponent>>())
            {
                query.ValueRW.direction = (_xAxis != 0 || _zAxis != 0)
                    ? new float3(_xAxis, YAxis, _zAxis)
                    : Vector3.zero;
            }
        }
    }
}

// if (Physics.Raycast(CameraSingleton.Instance.Camera.ScreenPointToRay(touch.position), out var hit))
// {
//     Debug.Log(hit.collider.gameObject.name);
//
//
//     //   foreach (var cursor in SystemAPI.Query<RefRW<CursorComponent>>())
//     //   {
//     //        cursor.ValueRW.cursorPosition = hit.point;
//     // }
// }