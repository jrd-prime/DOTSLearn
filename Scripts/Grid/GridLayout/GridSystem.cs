using Grid.Points;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Grid.GridLayout
{
    public partial struct GridSystem : ISystem
    {
        private NativeList<PointComponent> _tempPointsList;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var em = state.EntityManager;

            var gridEntity = SystemAPI.GetSingletonEntity<GridComponent>();
            var gridComponent = em.GetComponentData<GridComponent>(gridEntity);
            em.AddComponent<GridData>(gridEntity);

            _tempPointsList = new NativeList<PointComponent>(Allocator.Temp);

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            GeneratePoints(ecb, gridComponent, ref state);
            // ecb final
            ecb.Playback(em);
            ecb.Dispose();

            // set data
            em.SetComponentData(gridEntity, new GridData { PointsData = _tempPointsList });
            _tempPointsList.Dispose();
        }


        private void GeneratePoints(EntityCommandBuffer ecb, GridComponent gc, ref SystemState state)
        {
            var gridSize = gc.gridSize;
            var mainPrefab = gc.pointPrefabMain;
            var midPrefab = gc.pointPrefabMid;
            var smallPrefab = gc.pointPrefabSmall;

            // Main grid
            var mainStart = new float2(0f, 0f);
            GeneratePointsLevel(mainStart, gridSize, mainPrefab, 0.1f, ecb, ref state);

            // Middle grid
            var midStart = new float2(0.5f, 0.5f);
            var midSize = new int2(gridSize.x - 1, gridSize.y - 1);
            GeneratePointsLevel(midStart, midSize, midPrefab, 0.05f, ecb, ref state);

            // Small grid
            const float defOff = 0.375f;
            const float smallOffset = 0.25f;
            const float smallScale = 0.02f;
            var smallSize = new int2(gridSize.x - 1, gridSize.y - 1);

            var dot1 = new float2(defOff, defOff);
            var dot2 = new float2(defOff + smallOffset, defOff);
            var dot3 = new float2(defOff, defOff + smallOffset);
            var dot4 = new float2(defOff + smallOffset, defOff + smallOffset);

            GeneratePointsLevel(dot1, smallSize, smallPrefab, smallScale, ecb, ref state);
            GeneratePointsLevel(dot2, smallSize, smallPrefab, smallScale, ecb, ref state);
            GeneratePointsLevel(dot3, smallSize, smallPrefab, smallScale, ecb, ref state);
            GeneratePointsLevel(dot4, smallSize, smallPrefab, smallScale, ecb, ref state);
        }


        private void GeneratePointsLevel(float2 start, int2 size, Entity prefab, float scale, EntityCommandBuffer ecb,
            ref SystemState state)
        {
            var em = state.EntityManager;

            for (var x = start.x; x < size.x; x++)
            {
                for (var z = start.y; z < size.y; z++)
                {
                    var position = new float3(x, 0, z);
                    var entity = em.Instantiate(prefab);

                    ecb.AddComponent(entity, typeof(PointComponent));

                    var point = new PointComponent
                    {
                        id = _tempPointsList.Length,
                        pointPosition = position,
                        isBlocked = false,
                        self = entity
                    };
                    ecb.SetComponent(entity, new LocalTransform
                    {
                        Position = position,
                        Rotation = Quaternion.identity,
                        Scale = scale
                    });
                    ecb.SetComponent(entity,point);

                    _tempPointsList.Add(point);
                }
            }
        }
    }
}