using Jrd.Grid.GridLayout;
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
            state.RequireForUpdate<GridComponent>();
            state.RequireForUpdate<CursorComponent>();
            state.RequireForUpdate<GridData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var gridDataEntity = SystemAPI.GetSingletonEntity<GridData>();
            var gridData = em.GetComponentData<GridData>(gridDataEntity);
            var cursorEntity = SystemAPI.GetSingletonEntity<CursorComponent>();
            var cursor = em.GetComponentData<CursorComponent>(cursorEntity);
            var gridEntity = SystemAPI.GetSingletonEntity<GridComponent>();
            var grid = em.GetComponentData<GridComponent>(gridEntity);

            // round cursor coords
            var coords = new float3(
                Mathf.Round(cursor.cursorPosition.x),
                Mathf.Round(cursor.cursorPosition.y),
                Mathf.Round(cursor.cursorPosition.z));

            // temp checkbox
            if (!grid.findPoints) return;
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