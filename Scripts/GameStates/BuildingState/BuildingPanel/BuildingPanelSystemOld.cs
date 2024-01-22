// using Jrd.GameStates.BuildingState.ConfirmationPanel;
// using Jrd.GameStates.BuildingState.Prefabs;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using UnityEngine;
//
// namespace Jrd.GameStates.BuildingState.BuildingPanel
// {
//     [UpdateAfter(typeof(BuildingStateSystem))]
//     public partial class BuildingPanelSystemOld : SystemBase
//     {
//         private Entity _buildPrefabsComponent;
//         private EntityCommandBuffer _bsEcb;
//         private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
//
//         protected override void OnCreate()
//         {
//             RequireForUpdate<BuildPrefabsComponent>();
//             RequireForUpdate<ConfirmationPanelData>();
//         }
//
//         protected override void OnStartRunning()
//         {
//             _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
//         }
//
//         protected override void OnUpdate()
//         {
//             _bsEcb = _ecbSystem.CreateCommandBuffer(World.Unmanaged);
//
//             // SHOW // LOOK подумать, т.к. в каждой панели будет +- такие же шоу/хайд
//             foreach (var (buildingPanel, visibility, entity) in SystemAPI
//                          .Query<RefRO<BuildingPanelComponent>, RefRW<VisibilityComponent>>()
//                          .WithAll<BuildingPanelComponent, ShowVisualElementTag>()
//                          .WithEntityAccess())
//             {
//                 Debug.Log("show bpanel");
//                 _bsEcb.RemoveComponent<ShowVisualElementTag>(entity);
//
//                 var names = new NativeList<FixedString32Bytes>(buildingPanel.ValueRO.BuildingPrefabsCount,
//                     Allocator.Temp);
//                 names.Add("1x1");
//                 names.Add("2x2");
//                 names.Add("coll");
//                 names.Add("coll+rig");
//                 names.Add("coll+rig+kin");
//
//                 UI_old.BuildingPanelUI.InstantiateButtons(buildingPanel.ValueRO.BuildingPrefabsCount, names
//                 );
//                 // UI_old.BuildingPanelUI.ShowBPanel();
//                 names.Dispose();
//
//                 visibility.ValueRW.IsVisible = true;
//             }
//
//             // HIDE // LOOK подумать, т.к. в каждой панели будет +- такие же шоу/хайд
//             foreach (var (q, entity) in SystemAPI.Query<RefRW<VisibilityComponent>>()
//                          .WithAll<BuildingPanelComponent, HideVisualElementTag>()
//                          .WithEntityAccess())
//             {
//                 Debug.Log("hide");
//                 _bsEcb.RemoveComponent<HideVisualElementTag>(entity);
//
//                 // UI_old.BuildingPanelUI.HideBPanel();
//
//                 q.ValueRW.IsVisible = false;
//             }
//         }
//     }
// }