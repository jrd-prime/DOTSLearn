using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Screen
{
    public partial struct ClickToRaySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // Only 1 click
            if (Input.touchCount != 1 || Input.touches[0].phase is not TouchPhase.Began)
                return; // TODO refact

            foreach (var inputCursorData in SystemAPI.Query<RefRW<InputCursorData>>())
            {
                Debug.Log("Ray");
                Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(inputCursorData.ValueRO.CursorScreenPosition);
                inputCursorData.ValueRW.ClickToRay = ray;
            }
        }
    }
}