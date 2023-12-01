using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct GridLayoutSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridLayoutComponent>();
            // grid holder
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var em = state.EntityManager;

            var pointArchetype = em.CreateArchetype(
                typeof(PointComponent),
                typeof(LocalTransform)
            );

            var gridLayoutEntity = SystemAPI.GetSingletonEntity<GridLayoutComponent>();
            var gridLayoutComponent = em.GetComponentData<GridLayoutComponent>(gridLayoutEntity);


            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            GeneratePoints(em, ecb, gridLayoutComponent, ref state);

            ecb.Playback(em);
            ecb.Dispose();
        }


        private void GeneratePoints(EntityManager em, EntityCommandBuffer ecb, GridLayoutComponent gridLayoutComponent,
            ref SystemState state)
        {
            var gridSize = gridLayoutComponent.gridSize;
            var mainPrefab = gridLayoutComponent.pointPrefabMain;
            var middlePrefab = gridLayoutComponent.pointPrefabMid;
            var smallPrefab = gridLayoutComponent.pointPrefabSmall;

            // Main grid
            GeneratePointsLevel(new float2(0f, 0f), gridSize, mainPrefab, 0.1f, em, ecb, ref state);

            // Middle grid
            GeneratePointsLevel(new float2(0.5f, 0.5f), new int2(gridSize.x - 1, gridSize.y - 1),
                middlePrefab,
                0.05f, em, ecb, ref state);

            // Small grid

            const float defOff = 0.375f;
            const float smallPointOffset = 0.25f;
            const float smallPrefabScale = 0.02f;
            var smallPointsLevelGridSize = new int2(gridSize.x - 1, gridSize.y - 1);

            var dot1 = new float2(defOff, defOff);
            var dot2 = new float2(defOff + smallPointOffset, defOff);
            var dot3 = new float2(defOff, defOff + smallPointOffset);
            var dot4 = new float2(defOff + smallPointOffset, defOff + smallPointOffset);

            GeneratePointsLevel(dot1, smallPointsLevelGridSize, smallPrefab, smallPrefabScale, em, ecb, ref state);
            GeneratePointsLevel(dot2, smallPointsLevelGridSize, smallPrefab, smallPrefabScale, em, ecb, ref state);
            GeneratePointsLevel(dot3, smallPointsLevelGridSize, smallPrefab, smallPrefabScale, em, ecb, ref state);
            GeneratePointsLevel(dot4, smallPointsLevelGridSize, smallPrefab, smallPrefabScale, em, ecb, ref state);
        }


        private void GeneratePointsLevel(float2 startPosition, int2 gridSize, Entity pointPrefab, float prefabScale,
            EntityManager em, EntityCommandBuffer ecb,
            ref SystemState state)
        {
            for (var m = startPosition.x; m < gridSize.x; m++)
            {
                for (var n = startPosition.y; n < gridSize.y; n++)
                {
                    var c = em.Instantiate(pointPrefab);
                    ecb.AddComponent(c, typeof(LocalTransform));
                    ecb.AddComponent(c, typeof(PointComponent));

                    SystemAPI.SetComponent(c, new LocalTransform
                    {
                        Position = new float3(m, 0, n),
                        Rotation = Quaternion.identity,
                        Scale = prefabScale
                    });
                }
            }
        }
    }
}