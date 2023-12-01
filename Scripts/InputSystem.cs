using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Устанавливает вектор движения (без высоты)
    /// (пока)
    /// </summary>
    public partial struct InputSystem : ISystem
    {
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";

        public void OnCreate(ref SystemState state)
        {
            Debug.Log("InputSystem");
        }

        public void OnUpdate(ref SystemState state)
        {
            var horizontalAxis = Input.GetAxis(Horizontal);
            var verticalAxis = Input.GetAxis(Vertical);
            const float y = 0f;

            foreach (var query in SystemAPI.Query<RefRW<InputEventComponent>>())
            {
                Debug.Log("InputEvet + direction");
                query.ValueRW.direction = new float3(horizontalAxis, y, verticalAxis);
            }
        }
    }
}