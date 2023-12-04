using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.UserInput
{
    /// <summary>
    /// Устанавливает вектор движения (пока просто wasd)
    /// </summary>
    public partial struct InputSystem : ISystem
    {
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";

        public void OnUpdate(ref SystemState state)
        {
            // var horizontalAxis = Input.GetAxisRaw(Horizontal);
            // var verticalAxis = Input.GetAxisRaw(Vertical);
            
            var horizontalAxis = Input.GetAxis(Horizontal);
            var verticalAxis = Input.GetAxis(Vertical);
            const float y = 0f;

            foreach (var query in SystemAPI.Query<RefRW<InputEventComponent>>())
            {
                query.ValueRW.direction = (horizontalAxis != 0 || verticalAxis != 0)
                    ? new float3(horizontalAxis, y, verticalAxis)
                    : Vector3.zero;
            }
        }
    }
}