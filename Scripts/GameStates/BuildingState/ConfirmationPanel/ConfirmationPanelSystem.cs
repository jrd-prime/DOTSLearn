using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.JUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.ConfirmationPanel
{
    [UpdateBefore(typeof(BuildingStateSystem))]
    public partial class ConfirmationPanelSystem : SystemBase
    {
        private Entity _buildPrefabsComponent;

        protected override void OnCreate()
        {
            RequireForUpdate<BuildPrefabsComponent>();
        }

        protected override void OnStartRunning()
        {
            _buildPrefabsComponent = SystemAPI.GetSingletonEntity<BuildPrefabsComponent>();
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // SHOW // LOOK подумать, т.к. в каждой панели будет +- такие же шоу/хайд
            foreach (var (buildingPanelComponent, visibilityComponent, entity) in SystemAPI
                         .Query<RefRO<ConfirmationPanelTag>, RefRW<VisibilityComponent>>()
                         .WithAll<ConfirmationPanelTag, ShowVisualElementTag>()
                         .WithEntityAccess())
            {
                ConfirmationPanelUI.ShowApplyPanel();
                visibilityComponent.ValueRW.IsVisible = true;
                ecb.RemoveComponent<ShowVisualElementTag>(entity);
            }

            // HIDE // LOOK подумать, т.к. в каждой панели будет +- такие же шоу/хайд
            foreach (var (q, entity) in SystemAPI.Query<RefRW<VisibilityComponent>>()
                         .WithAll<ConfirmationPanelTag, HideVisualElementTag>()
                         .WithEntityAccess())
            {
                Debug.Log("hide");
                ecb.RemoveComponent<HideVisualElementTag>(entity);
                q.ValueRW.IsVisible = false;
                ConfirmationPanelUI.HideApplyPanel();
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}