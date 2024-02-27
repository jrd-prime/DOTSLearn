// using Sources.Scripts.CommonComponents.Building;
// using Unity.Collections;
// using Unity.Entities;
//
// namespace Sources.Scripts.Game.Common.SaveAndLoad
// {
//     public partial struct GameBuildingsMapSystem : ISystem
//     {
//         private RefRW<GameBuildingsMapData> _gameBuildingsData;
//
//         public void OnCreate(ref SystemState state)
//         {
//             var entityManager = state.EntityManager;
//             var e = entityManager.CreateEntity();
//             entityManager.AddComponent<GameBuildingsMapData>(e);
//             entityManager.SetName(e, "___ GAME BUILDINGS");
//         }
//
//         public void OnUpdate(ref SystemState state)
//         {
//             state.Enabled = false;
//
//             _gameBuildingsData = SystemAPI.GetSingletonRW<GameBuildingsMapData>();
//
//             if (!_gameBuildingsData.ValueRO.GameBuildings.IsCreated)
//             {
//                 _gameBuildingsData.ValueRW.GameBuildings =
//                     new NativeHashMap<FixedString64Bytes, BuildingData>(1, Allocator.Persistent);
//             }
//         }
//
//         public void OnDestroy(ref SystemState state)
//         {
//             _gameBuildingsData = SystemAPI.GetSingletonRW<GameBuildingsMapData>();
//             _gameBuildingsData.ValueRW.GameBuildings.Dispose();
//         }
//     }
// }