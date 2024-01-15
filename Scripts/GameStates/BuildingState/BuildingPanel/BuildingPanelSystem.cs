﻿using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.JUI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.BuildingPanel
{
    [UpdateAfter(typeof(BuildingStateSystem))]
    public partial class BuildingPanelSystem : SystemBase
    {
        private Entity _buildPrefabsComponent;
        private EntityCommandBuffer _eiEcb;
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;

        protected override void OnCreate()
        {
            RequireForUpdate<BuildPrefabsComponent>();
            RequireForUpdate<ConfirmationPanelTag>();
        }

        protected override void OnStartRunning()
        {
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
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
                Debug.Log("show bpanel");
                _eiEcb.RemoveComponent<ShowVisualElementTag>(entity);

                var names = new NativeList<FixedString32Bytes>(buildingPanelComponent.ValueRO.BuildingPrefabsCount,
                    Allocator.Temp);
                names.Add("1x1");
                names.Add("2x2");
                names.Add("coll");
                names.Add("coll+rig");
                names.Add("coll+rig+kin");

                BuildingPanelUI.InstantiateButtons(buildingPanelComponent.ValueRO.BuildingPrefabsCount, names
                );
                BuildingPanelUI.ShowBPanel();
                names.Dispose();

                visibilityComponent.ValueRW.IsVisible = true;
            }

            // HIDE // LOOK подумать, т.к. в каждой панели будет +- такие же шоу/хайд
            foreach (var (q, entity) in SystemAPI.Query<RefRW<VisibilityComponent>>()
                         .WithAll<BuildingPanelComponent, HideVisualElementTag>()
                         .WithEntityAccess())
            {
                Debug.Log("hide");
                _eiEcb.RemoveComponent<HideVisualElementTag>(entity);

                BuildingPanelUI.HideBPanel();

                q.ValueRW.IsVisible = false;
            }
        }
    }
}