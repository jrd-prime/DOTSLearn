﻿// using System.Threading.Tasks;
// using Sources.Scripts.CommonComponents.Building;
// using Sources.Scripts.Game.Features.Building;
// using Unity.Collections;
// using Unity.Entities;
// using BuildingDataAspect = Sources.Scripts.CommonComponents.Building.BuildingDataAspect;
//
// namespace Sources.Scripts.Game.Common.SaveAndLoad
// {
//     /// <summary>
//     /// Add new placed building to list/map/db/etc
//     /// </summary>
//     public partial struct AddBuildingToMapSystem : ISystem
//     {
//         private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
//         private EntityCommandBuffer _bsEcb;
//         private NativeHashMap<FixedString64Bytes, BuildingData> _gameBuildingsHashMap;
//
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
//         }
//
//         public void OnUpdate(ref SystemState state)
//         {
//             _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
//             _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
//
//             _gameBuildingsHashMap = SystemAPI.GetSingletonRW<GameBuildingsMapData>().ValueRW.GameBuildings;
//
//             foreach (var (buildingData, entity) in SystemAPI
//                          .Query<BuildingDataAspect>()
//                          .WithAll<AddBuildingToDBTag>()
//                          .WithEntityAccess())
//             {
//                 AddToHashMap(buildingData.BuildingData);
//
//                 _bsEcb.RemoveComponent<AddBuildingToDBTag>(entity);
//                 _bsEcb.AddComponent<SaveBuildingToDBTag>(entity);
//             }
//         }
//
//         private async void AddToHashMap(BuildingData buildingData)
//         {
//             NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsHashMap = _gameBuildingsHashMap;
//             
//             await Task.Run(() =>
//             {
//                 // Debug.LogWarning("add me " + buildingData.Guid);
//
//                 if (!gameBuildingsHashMap.ContainsKey(buildingData.Guid))
//                 {
//                     gameBuildingsHashMap.Add(buildingData.Guid, buildingData);
//                 }
//             });
//         }
//     }
// }