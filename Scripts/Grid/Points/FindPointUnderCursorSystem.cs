using Jrd.DebSet;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Jrd.UserInput;

namespace Jrd.Grid.Points
{
    public partial struct FindPointUnderCursorSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<GridData>();
            state.RequireForUpdate<CursorComponent>();
            // state.RequireForUpdate<GridComponent>();
            state.RequireForUpdate<DebSetComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            // var gridData = em.GetComponentData<GridData>(SystemAPI.GetSingletonEntity<GridData>());
            var cursor = em.GetComponentData<CursorComponent>(SystemAPI.GetSingletonEntity<CursorComponent>());
            // var grid = em.GetComponentData<GridComponent>(SystemAPI.GetSingletonEntity<GridComponent>());
            var debSet = em.GetComponentData<DebSetComponent>(SystemAPI.GetSingletonEntity<DebSetComponent>());

            // round cursor coords
            var coords = new float3(
                Mathf.Round(cursor.cursorPosition.x),
                Mathf.Round(cursor.cursorPosition.y),
                Mathf.Round(cursor.cursorPosition.z));

            // temp checkbox
            if (!debSet.mouseRaycast) return;
            foreach (var point in SystemAPI.Query<RefRW<PointComponent>, RefRO<PointMainTagComponent>>())
            {
                var p = point.Item1.ValueRO;
                if (Equals(coords, p.pointPosition))
                {
                    // temp scale +
                    em.SetComponentData(p.self, new LocalTransform
                    {
                        Position = coords,
                        Scale = .2f,
                        Rotation = Quaternion.identity
                    });
                }
            }
        }
    }
}