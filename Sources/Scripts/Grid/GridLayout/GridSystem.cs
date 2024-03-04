// using Sources.Scripts.Grid.Points;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Sources.Scripts.Grid.GridLayout
// {
//     // TODO https://app.asana.com/0/1206217975075068/1206234441074580/f
//     public partial struct GridSystem : ISystem
//     {
//         // TODO CACHE
//         private NativeList<PointComponent> _tempPointsList;
//
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
//             state.RequireForUpdate<GridComponent>();
//             state.Enabled = false;
//         }
//
//         public void OnUpdate(ref SystemState state)
//         {
//             var biEcb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
//                 .CreateCommandBuffer(state.WorldUnmanaged);
//
//             var em = state.EntityManager;
//
//             var gridEntity = SystemAPI.GetSingletonEntity<GridComponent>();
//             var gridComponent = em.GetComponentData<GridComponent>(gridEntity);
//             biEcb.AddComponent<GridData>(gridEntity);
//
//             // _tempPointsList = new NativeList<PointComponent>(Allocator.Temp);
//
//             // 5tests 100x100 ticks / mills 243 + 230 + 477 + 213 + 207
//             // 5tests 1000x1000 ticks / mills 21870 + 21250 + 21930 + 21200 + 21250
//
//             _tempPointsList = new NativeList<PointComponent>(1, Allocator.TempJob);
//
//             // var stopwatch = Stopwatch.StartNew();
//
//             var gridSize = gridComponent.gridSize;
//             var mainPrefab = gridComponent.pointPrefabMain;
//             var midPrefab = gridComponent.pointPrefabMid;
//             var smallPrefab = gridComponent.pointPrefabSmall;
//
//             // Main grid
//
//             var mainStart = new float2(0f, 0f);
//             var j1 = new GeneratePointsJob
//                 {
//                     Tag = 1,
//                     Start = mainStart,
//                     Size = gridSize,
//                     Prefab = mainPrefab,
//                     Scale = 0.1f,
//                     Ecb = biEcb,
//                     TempPointsList = _tempPointsList
//                 }
//                 .Schedule(state.Dependency);
//
//
//             // Middle grid
//
//
//             var midStart = new float2(0.5f, 0.5f);
//             var midSize = new int2(gridSize.x - 1, gridSize.y - 1);
//             var j2 = new GeneratePointsJob
//                 {
//                     Tag = 2,
//                     Start = midStart,
//                     Size = midSize,
//                     Prefab = midPrefab,
//                     Scale = 0.05f,
//                     Ecb = biEcb,
//                     TempPointsList = _tempPointsList
//                 }
//                 .Schedule(j1);
//
//
//             // Small grid
//
//             const float defOff = 0.375f;
//             const float smallOffset = 0.25f;
//             const float smallScale = 0.02f;
//             var smallSize = new int2(gridSize.x - 1, gridSize.y - 1);
//             var dot1 = new float2(defOff, defOff);
//             var dot2 = new float2(defOff + smallOffset, defOff);
//             var dot3 = new float2(defOff, defOff + smallOffset);
//             var dot4 = new float2(defOff + smallOffset, defOff + smallOffset);
//
//             var j3 = new GeneratePointsJob
//                 {
//                     Tag = 3,
//                     Start = dot1,
//                     Size = smallSize,
//                     Prefab = smallPrefab,
//                     Scale = smallScale,
//                     Ecb = biEcb,
//                     TempPointsList = _tempPointsList
//                 }
//                 .Schedule(j2);
//
//             var j4 = new GeneratePointsJob
//                 {
//                     Tag = 3,
//                     Start = dot2,
//                     Size = smallSize,
//                     Prefab = smallPrefab,
//                     Scale = smallScale,
//                     Ecb = biEcb,
//                     TempPointsList = _tempPointsList
//                 }
//                 .Schedule(j3);
//
//             var j5 = new GeneratePointsJob
//                 {
//                     Tag = 3,
//                     Start = dot3,
//                     Size = smallSize,
//                     Prefab = smallPrefab,
//                     Scale = smallScale,
//                     Ecb = biEcb,
//                     TempPointsList = _tempPointsList
//                 }
//                 .Schedule(j4);
//
//             var j6 = new GeneratePointsJob
//                 {
//                     Tag = 3,
//                     Start = dot4,
//                     Size = smallSize,
//                     Prefab = smallPrefab,
//                     Scale = smallScale,
//                     Ecb = biEcb,
//                     TempPointsList = _tempPointsList
//                 }
//                 .Schedule(j5);
//
//             j6.Complete();
//
//             // stopwatch.Stop();
//             // Debug.Log(stopwatch.ElapsedTicks);
//             // Debug.Log(stopwatch.ElapsedMilliseconds);
//
//             // set data
//             biEcb.SetComponent(gridEntity, new GridData { PointsData = _tempPointsList });
//             _tempPointsList.Dispose();
//         }
//
//         // LOOK USE
//         // var result = new NativeArray<int>(1, Allocator.TempJob);
//         //
//         // Job.WithCode(() =>
//         // {
//         //     for (int i = 1; i <= 10; i++)
//         //     {
//         //         numbers[0] += i;
//         //     }
//         // }).Schedule();
//         //
//         // result.Dispose();
//
//         [BurstCompile]
//         private struct GeneratePointsJob : IJob // переделать,стремно выглядит
//         {
//             public int Tag;
//             public float2 Start;
//             public int2 Size;
//             public Entity Prefab;
//             public float Scale;
//             public EntityCommandBuffer Ecb;
//             public NativeList<PointComponent> TempPointsList;
//
//             public void Execute()
//             {
//                 for (var x = Start.x; x < Size.x; x++)
//                 {
//                     for (var z = Start.y; z < Size.y; z++)
//                     {
//                         var position = new float3(x, 0, z);
//                         var entity = Ecb.Instantiate(Prefab);
//
//                         switch (Tag)
//                         {
//                             case 1: // main
//                                 Ecb.AddComponent<PointMainTagComponent>(entity);
//                                 break;
//                             case 2: // mid
//                                 Ecb.AddComponent<PointMidTagComponent>(entity);
//                                 break;
//                             case 3: // small
//                                 Ecb.AddComponent<PointSmallTagComponent>(entity);
//                                 break;
//                         }
//
//                         Ecb.SetComponent(entity,
//                             new LocalTransform
//                             {
//                                 Position = position,
//                                 Rotation = Quaternion.identity,
//                                 Scale = Scale
//                             });
//
//                         Ecb.AddComponent(entity,
//                             new PointComponent
//                             {
//                                 id = TempPointsList.Length,
//                                 pointPosition = position,
//                                 isBlocked = false,
//                                 self = entity,
//                                 prefab = Prefab
//                             });
//
//                         // TempPointsList.Add(point);
//                     }
//                 }
//             }
//         }
//     }
// }