using System.Collections.Generic;
using Jrd.JCamera;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jrd.UserInput
{
    /// <summary>
    /// Устанавливает вектор движения (пока просто wasd)
    /// </summary>
    public partial struct InputSystem : ISystem
    {
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";

        private float xAxis;
        const float yAxis = 0f;
        private float zAxis;


        public void OnUpdate(ref SystemState state)
        {
            // var horizontalAxis = Input.GetAxisRaw(Horizontal);
            // var verticalAxis = Input.GetAxisRaw(Vertical);
#if UNITY_EDITOR

            xAxis = Input.GetAxis(Horizontal);
            zAxis = Input.GetAxis(Vertical);

#endif
#if UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);


                var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
                };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                Debug.Log(results.Count > 0);

                if (results.Count > 0)
                {
                }
                else
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        var posDelta = touch.deltaPosition * SystemAPI.Time.DeltaTime * -1;
                        xAxis = posDelta.x;
                        zAxis = posDelta.y;
                    }
                    else
                    {
                        xAxis = 0;
                        zAxis = 0;
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
            }

#endif

            foreach (var query in SystemAPI.Query<RefRW<InputEventComponent>>())
            {
                query.ValueRW.direction = (xAxis != 0 || zAxis != 0)
                    ? new float3(xAxis, yAxis, zAxis)
                    : Vector3.zero;
            }
        }

        private bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}