using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Jrd.UserInput.Components;
using Jrd.Utils.DebSet;

namespace Jrd.Grid.Points
{
    public partial struct FindPointUnderCursorSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<GridComponent>();
            // state.RequireForUpdate<GridData>();
            state.RequireForUpdate<InputCursorData>();
            state.RequireForUpdate<DebSetComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var debSet = em.GetComponentData<DebSetComponent>(SystemAPI.GetSingletonEntity<DebSetComponent>());
            
            if (!debSet.MouseRaycast) return;
            
            var cursor = em.GetComponentData<InputCursorData>(SystemAPI.GetSingletonEntity<InputCursorData>());
            // var grid = em.GetComponentData<GridComponent>(SystemAPI.GetSingletonEntity<GridComponent>());
            // var gridData = em.GetComponentData<GridData>(SystemAPI.GetSingletonEntity<GridData>());

            // round cursor coords
            var coords = new float3(
                Mathf.Round(cursor.CursorWorldPosition.x),
                Mathf.Round(cursor.CursorWorldPosition.y),
                Mathf.Round(cursor.CursorWorldPosition.z));
            
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