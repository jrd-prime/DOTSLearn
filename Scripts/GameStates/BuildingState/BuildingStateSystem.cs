﻿using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Jrd.JUtils;
using Jrd.UI_old;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    [UpdateAfter(typeof(GameStatesSystem))]
    public partial class BuildingStateSystem : SystemBase
    {
        private EntityManager _em;
        private NativeList<Entity> _stateVisualComponents;
        private BeginSimulationEntityCommandBufferSystem.Singleton _bsEcbSystem;
        private BeginInitializationEntityCommandBufferSystem.Singleton _biEcbSystem;
        private EntityCommandBuffer _bsEcb;
        private EntityCommandBuffer _biEcb;

        private Entity _buildingPanel;
        private Entity _confirmationPanel;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;
        private int _tempSelectedBuildID;
        private Entity _buildPrefabsComponentEntity;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<GameStateData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _em = EntityManager;
        }

        protected override void OnStartRunning()
        {
            _tempSelectedBuildID = -1;

            if (!_stateVisualComponents.IsCreated)
                _stateVisualComponents = new NativeList<Entity>(Allocator.Persistent); // TODO подумать

            UI_old.BuildingPanel.OnBuildSelected += BuildSelected;
            ConfirmationPanelUI.ApplyPanelApplyButton.clicked += ConfirmBuilding;
            ConfirmationPanelUI.ApplyPanelCancelButton.clicked += CancelBuilding;
        }

        protected override void OnStopRunning()
        {
            _stateVisualComponents.Dispose();
            UI_old.BuildingPanel.OnBuildSelected -= BuildSelected;
            ConfirmationPanelUI.ApplyPanelApplyButton.clicked -= ConfirmBuilding;
            ConfirmationPanelUI.ApplyPanelCancelButton.clicked -= CancelBuilding;
        }

        protected override void OnUpdate()
        {
            _gameStateData = SystemAPI.GetSingletonRW<GameStateData>();

            _biEcbSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            _bsEcbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _bsEcbSystem.CreateCommandBuffer(World.Unmanaged);

            Debug.Log("err start");
            _buildPrefabsComponentEntity = SystemAPI.GetSingletonEntity<BuildPrefabsComponent>();

            SystemAPI.TryGetSingletonEntity<BuildPrefabsComponent>(out Entity ga);
            H.T(ga.ToString());
            Debug.Log("err stop");

            // Init by tag // LOOK TODO вытащить в отдельную систему обобщенную
            foreach (var (buildingStateComponent, entity) in SystemAPI
                         .Query<RefRW<BuildingStateComponent>>()
                         .WithAll<InitializeTag>()
                         .WithEntityAccess())
            {
                // Is init?
                if (buildingStateComponent.ValueRO.IsInitialized) return;

                _buildingPanel =
                    GetCustomEntityVisualElementComponent<BuildingPanelComponent>(BSConst.BuildingPanelEntityName);
                _bsEcb.AddComponent<ShowVisualElementTag>(_buildingPanel);
                _bsEcb.SetComponent(_buildingPanel,
                    new BuildingPanelComponent
                    {
                        BuildingPrefabsCount =
                            SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsComponentEntity).Length
                    });

                _confirmationPanel =
                    GetCustomEntityVisualElementComponent<ConfirmationPanelTag>(
                        BSConst.ConfirmationPanelEntityName);

                if (_stateVisualComponents.Length == 0)
                    Debug.LogWarning("We have a problem with create entities for Building State");

                _bsEcb.RemoveComponent<InitializeTag>(entity);

                buildingStateComponent.ValueRW.Self = entity;
                buildingStateComponent.ValueRW.IsInitialized = true;
            }
        }

        private void ConfirmBuilding()
        {
            Debug.Log("confirm  build");

            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempBuildingEntity))
            {
                _bsEcb.AddComponent<PlaceTempBuildingTag>(tempBuildingEntity);

                UI_old.BuildingPanel.SetButtonEnabled(_tempSelectedBuildID, true);
                _tempSelectedBuildID = -1;
            }
            else
            {
                Debug.LogWarning("We can't find temp building entity!");
            }
        }

        private void CancelBuilding()
        {
            Debug.Log("cancel build");

            DestroyTempPrefab();
            _tempSelectedBuildID = -1;
        }

        private void DestroyTempPrefab()
        {
            _biEcb = _biEcbSystem.CreateCommandBuffer(World.Unmanaged);

            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempEntity))
            {
                _biEcb.AddComponent(tempEntity, new DestroyTempPrefabTag());
            }
        }

        private void BuildSelected(Button button, int index)
        {
            if (_tempSelectedBuildID == -1)
            {
                _tempSelectedBuildID = index;
            }
            else if (_tempSelectedBuildID != index)
            {
                DestroyTempPrefab();
                _tempSelectedBuildID = index;
            }
            // temp = index
            else
            {
                Debug.LogWarning("We have a problem with enable/disable buttons in BuildPanel." + this);
            }

            var prefabElements = SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsComponentEntity);

            if (!prefabElements.IsEmpty)
            {
                _bsEcb.AddComponent<ShowVisualElementTag>(_confirmationPanel);
                _bsEcb.AddComponent(_gameStateData.ValueRO.BuildingStateEntity,
                    new InstantiateTempPrefabComponent
                    {
                        Prefab = prefabElements[index].PrefabEntity,
                        Name = prefabElements[index].PrefabName
                    });

                Debug.Log(
                    $"Build Selected. ID: {index} / Btn: {button.name} / Prefab: {prefabElements[index].PrefabName}");
                return;
            }

            Debug.LogError("Prefabs: " + prefabElements.Length);
        }

        private Entity GetCustomEntityVisualElementComponent<T>(FixedString64Bytes entityName)
            where T : unmanaged, IComponentData
        {
            var entity = _em.CreateEntity(); // TODO
            _stateVisualComponents.Add(entity);

            _bsEcb.AddComponent<T>(entity);
            _bsEcb.AddComponent<VisibilityComponent>(entity);

            _bsEcb.SetComponent(entity, new VisibilityComponent { IsVisible = false });

            _bsEcb.SetName(entity, $"{BSConst.Prefix} {entityName}");

            return entity;
        }
    }
}