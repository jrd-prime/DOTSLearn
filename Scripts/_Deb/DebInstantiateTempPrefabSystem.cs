// using Jrd._Deb;
// using Jrd.GameStates.BuildingState.Prefabs;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Jrd.GameStates.BuildingState.TempBuilding
// {
//     // [BurstCompile]
//     public partial struct DebInstantiateTempPrefabSystem : ISystem
//     {
//         // [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
//         }
//
//         // [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             state.Enabled = false;
//
//             var textPrefab = SystemAPI.GetSingletonBuffer<DebPrefabBufferElements>().ElementAt(0).PrefabEntity;
//
//             // var textPrefab = buffer.ElementAt(0);
//
//             var ecb = SystemAPI
//                 .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
//                 .CreateCommandBuffer(state.WorldUnmanaged);
//
//             var b = SystemAPI.GetSingletonBuffer<PrefabBufferElements>();
//
//             var c = new NativeList<float3>(b.Length, Allocator.TempJob);
//
//             c.Add(new float3(1, 0, 1));
//             c.Add(new float3(3, 0, 2));
//             c.Add(new float3(3, 0, 6));
//             c.Add(new float3(6, 0, 7));
//             c.Add(new float3(0, 0, 4));
//
//             // for (int i = 0; i < b.Length; i++)
//             // {
//             //     var entity = state.EntityManager.CreateEntity();
//             //     state.Dependency = new InstantiateTempPrefabJob
//             //         {
//             //             TempPrefabEntity = b[i].PrefabEntity,
//             //             TempPrefabName = b[i].PrefabName,
//             //             BsEcb = ecb,
//             //             BuildingStateEntity = entity,
//             //             Position = c.ElementAt(i),
//             //             Rotation = quaternion.identity,
//             //             Scale = 1
//             //         }
//             //         .Schedule(state.Dependency);
//             // }
//         }
//
//         // [BurstCompile]
//         private struct InstantiateTempPrefabJob : IJob
//         {
//             public EntityCommandBuffer BsEcb;
//             public Entity TempPrefabEntity;
//             public FixedString64Bytes TempPrefabName;
//             public Entity BuildingStateEntity;
//             public float3 Position;
//             public quaternion Rotation;
//             public float Scale;
//
//             // [BurstCompile]
//             public void Execute()
//             {
//                 // instantiate selected building prefab
//                 Entity instance = BsEcb.Instantiate(TempPrefabEntity);
//
//                 // set position // TODO
//                 BsEcb.SetComponent(instance, new LocalTransform
//                 {
//                     Position = Position,
//                     Rotation = Rotation,
//                     Scale = Scale
//                 });
//
//                 // name
//                 BsEcb.SetName(instance, "___ " + TempPrefabName);
//             }
//         }
//     }
// }