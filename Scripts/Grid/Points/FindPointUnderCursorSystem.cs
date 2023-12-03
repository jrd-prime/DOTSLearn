using System;
using Grid.GridLayout;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UserInput;

namespace Grid.Points
{
    public partial struct FindPointUnderCursorSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
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

            // round cursor coords
            var coords = new float3(
                Mathf.Round(cursor.cursorPosition.x),
                Mathf.Round(cursor.cursorPosition.y),
                Mathf.Round(cursor.cursorPosition.z));

            // temp checkbox
            var buildState = true;
            if (buildState)
            {
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
}