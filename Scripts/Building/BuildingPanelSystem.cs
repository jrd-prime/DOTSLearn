// using Jrd.Grid.Points;
// using Jrd.JCamera;
// using Jrd.JUI.EditModeUI;
// using Jrd.States;
// using Jrd.UserInput;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.UIElements;
// using UnityEngine.UIElements.Experimental;
//
// namespace Jrd
// {
//     public partial class BuildingPanelSystem : SystemBase
//     {
//         private EntityManager _em;
//         private Entity st;
//
//         private bool _isBuildingPanelInitialized;
//         private BuildingPrefabComponent _buildingPrefabComponent;
//
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<BuildingPrefabComponent>();
//         }
//
//         protected override void OnUpdate()
//         {
//             if (_isBuildingPanelInitialized) return;
//
//             _em = EntityManager;
//             var entity = SystemAPI.GetSingletonEntity<BuildingPrefabComponent>();
//             _buildingPrefabComponent = _em.GetComponentData<BuildingPrefabComponent>(entity);
//
//             st = _em.CreateEntity();
//             _em.SetName(st, "Edit Mode State");
//             _em.AddComponent(st, typeof(EditModeStateComponent));
//
//
//             BuildingPanelUI.BuildingCancel.clicked += () => H.T("BPU Cancel");
//             // BuildingPanelUI.Building1.clicked += ChoosePrefabForBuild;
//             BuildingPanelUI.Building2.clicked += () => H.T("BPU 2"); // dispose previous
//
//             // EditModeUI.EditModeCancelButton.clicked += () =>
//             // {
//             //     RemoveTempBuilding();
//             //     HideEditMode();
//             // };
//
//             _isBuildingPanelInitialized = true;
//         }
//
//         private void RemoveTempBuilding()
//         {
//             var ecb = new EntityCommandBuffer(Allocator.Temp);
//             foreach (var tempEntity in SystemAPI.Query<RefRO<TempBuildingPrefabTag>>())
//             {
//                 ecb.DestroyEntity(tempEntity.ValueRO.TempEntity);
//             }
//
//             _em.SetComponentData(st, new EditModeStateComponent { State = false });
//             
//             ecb.Playback(EntityManager);
//             ecb.Dispose();
//         }
//
//         // private void HideEditMode()
//         // {
//         //     if (EditModeUI.EditModeRoot.style.display == DisplayStyle.None) return;
//         //
//         //     var hideEditModePanelAnimation = EditModeUI.EditModePanel.experimental.animation.Start(
//         //         new StyleValues { bottom = 0f },
//         //         new StyleValues { bottom = -100f }, 500).Ease(Easing.InQuad).KeepAlive();
//         //     hideEditModePanelAnimation.onAnimationCompleted =
//         //         () => EditModeUI.EditModeRoot.style.display = DisplayStyle.None;
//         // }
//
//         // выбираем то что будем строить и происходит цепочка событий которая размещает префаб в центре
//         // экрана, записывает/читает данные по точкам
//         // и в итоге показывает интрефейс для мува строения
//         private void ChoosePrefabForBuild()
//         {
//             // LOOK здесь не место
//             _em.SetComponentData(st, new EditModeStateComponent { State = true });
//
//
//             H.T("ChoosePrefabForBuild");
//             Entity prefab = default; // TODO choose prefab
//
//             // LOOK show edit mode UI
//            // ShowEditModeUI(); // TODO to component
//
//             // LOOK test prefab
//             prefab = _buildingPrefabComponent.Building1Prefab;
//
//             // LOOK подумать и вытащить в отдельную систему и подсистемы,
//             // LOOK просто накидывать компоненты и тэги чтобы система подхватывала и в итоге было размещено
//             PlacePrefabInCenterScreen(prefab);
//
//             // после размещени строения на карте, надо:
//             // 1. камера должна фолоу за кордами префаба
//             // (например камера читает корды префаба с тэгом что это темповый объект)
//             // и держится от него на оффсете
//             // 2. инпут мувмента должен двигать не камеру а префаб
//         }
//
//         // private void ShowEditModeUI()
//         // {
//         //     if (EditModeUI.EditModeRoot.style.display == DisplayStyle.Flex) return;
//         //
//         //     EditModeUI.EditModeRoot.style.display = DisplayStyle.Flex;
//         //     var showEditMenuPanelAnimation = EditModeUI.EditModePanel.experimental.animation.Start(
//         //         new StyleValues { bottom = -100f },
//         //         new StyleValues { bottom = 0f }, 1000).Ease(Easing.OutElastic).KeepAlive();
//         //
//         //     // showEditMenuPanelAnimation.onAnimationCompleted = 
//         //
//         //     // EditModeUI.EditModeRoot.style.bottom = -100f;
//         //     // EditModeUI.EditModeRoot.experimental.animation
//         //     //     .Start(25f, 200f, 3000, (b, val) => { b.style.display = DisplayStyle.Flex; })
//         //     //     .Ease(Easing.OutBounce);
//         //
//         //     // a.onAnimationCompleted;
//         // }
//
//         private void PlacePrefabInCenterScreen(Entity prefab)
//         {
//             var screenCenterPoint = Utils.GetScreenCenterPoint();
//
//             var coords = GetPointCoordsInCenterScreen(screenCenterPoint);
//             var point = GetPointByCoords(coords);
//
//             if (point.self == Entity.Null) Debug.LogError("GetPointByCoords Entity NULL"); // ERROR
//
//             PlacePrefab(prefab, point);
//         }
//
//         private void PlacePrefab(Entity prefab, PointComponent point)
//         {
//             H.T("PlacePrefab");
//             var entity = _em.Instantiate(prefab);
//             _em.AddComponent<TargetComponent>(entity);
//             _em.SetComponentData(entity, new TargetComponent
//             {
//                 TargetPosition = point.pointPosition
//             });
//             _em.AddComponent<TempBuildingPrefabTag>(entity);
//             _em.AddComponent<MovingEventComponent>(entity);
//             _em.SetComponentData(entity, new TempBuildingPrefabTag
//             {
//                 TempEntity = entity
//             });
//             _em.SetComponentData(entity, new LocalTransform
//             {
//                 Position = point.pointPosition,
//                 Rotation = Quaternion.identity,
//                 Scale = 1
//             });
//         }
//
//         private PointComponent GetPointByCoords(float3 coords)
//         {
//             H.T("GetPointByCoords");
//             // LOOK TODO dublicate, findpointundercursorsystem. FIX
//             foreach (var point in SystemAPI.Query<RefRW<PointComponent>, RefRO<PointMainTagComponent>>())
//             {
//                 var p = point.Item1.ValueRO;
//                 if (Equals(coords, p.pointPosition))
//                 {
//                     H.T("Entity" + p.self);
//                     return p;
//                     // temp scale +
//                     // _em.SetComponentData(p.self, new LocalTransform
//                     // {
//                     //     Position = coords,
//                     //     Scale = .2f,
//                     //     Rotation = Quaternion.identity
//                     // });
//                 }
//             }
//
//             return new PointComponent { self = Entity.Null }; // LOOK FIX
//         }
//
//         private float3 GetPointCoordsInCenterScreen(Vector3 screenCenterPoint)
//         {
//             if (Physics.Raycast(CameraSingleton.Instance.Camera.ScreenPointToRay(screenCenterPoint), out var hit))
//             {
//                 return new float3
//                 (
//                     Mathf.Floor(hit.point.x),
//                     0,
//                     Mathf.Floor(hit.point.z)
//                 );
//             }
//
//             return float3.zero;
//         }
//     }
// }