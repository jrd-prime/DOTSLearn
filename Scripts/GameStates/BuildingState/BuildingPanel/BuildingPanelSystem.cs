using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.SharedComponentAndSystems;
using Jrd.JUI;
using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.BuildingPanel
{
    [UpdateAfter(typeof(BuildingStateSystem))]
    public partial class BuildingPanelSystem : SystemBase
    {
        private Entity _buildPrefabsComponent;
        private Entity _confirmationPanelEntity;
        private EntityCommandBuffer _eiEcb;
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;

        protected override void OnCreate()
        {
            RequireForUpdate<BuildPrefabsComponent>();
            RequireForUpdate<ConfirmationPanelComponent>();
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        protected override void OnStartRunning()
        {
            _confirmationPanelEntity = SystemAPI.GetSingletonEntity<ConfirmationPanelComponent>();
        }

        protected override void OnUpdate()
        {
            _eiEcb = _ecbSystem.CreateCommandBuffer(World.Unmanaged);

            // SHOW // LOOK подумать, т.к. в каждой панели будет +- такие же шоу/хайд
            foreach (var (buildingPanelComponent, visibilityComponent, entity) in SystemAPI
                         .Query<RefRO<BuildingPanelComponent>, RefRW<VisibilityComponent>>()
                         .WithAll<BuildingPanelComponent, ShowVisualElementTag>()
                         .WithEntityAccess())
            {
                Debug.Log(entity);
                BuildingPanelUI.InstantiateButtons(buildingPanelComponent.ValueRO.BuildingPrefabsCount);
                BuildingPanelUI.ShowBPanel();
                visibilityComponent.ValueRW.IsVisible = true;
                _eiEcb.RemoveComponent<ShowVisualElementTag>(entity);

            }

            // HIDE // LOOK подумать, т.к. в каждой панели будет +- такие же шоу/хайд
            foreach (var (q, entity) in SystemAPI.Query<RefRW<VisibilityComponent>>()
                         .WithAll<BuildingPanelComponent, HideVisualElementTag>()
                         .WithEntityAccess())
            {
                Debug.Log("hide");
                _eiEcb.RemoveComponent<HideVisualElementTag>(entity);
                q.ValueRW.IsVisible = false;
                BuildingPanelUI.HideBPanel();
            }
        }

        
    }
}