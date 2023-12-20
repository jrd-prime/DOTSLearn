using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Jrd.Screen;
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
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;

        private Entity _buildingPanel;
        private Entity _confirmationPanel;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;
        private int _tempSelectedBuildID;
        private Entity _buildPrefabsComponent;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<GameStateData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _em = EntityManager;
        }

        protected override void OnStartRunning()
        {
            _buildPrefabsComponent = SystemAPI.GetSingletonEntity<BuildPrefabsComponent>();
            _tempSelectedBuildID = -1;
        }

        protected override void OnUpdate()
        {
            if (!_stateVisualComponents.IsCreated)
                _stateVisualComponents = new NativeList<Entity>(2, Allocator.Persistent); // TODO подумать

            _gameStateEntity = SystemAPI.GetSingletonEntity<GameStateData>();
            _gameStateData = SystemAPI.GetComponentRW<GameStateData>(_gameStateEntity); // TODO aspect

            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _ecbSystem.CreateCommandBuffer(World.Unmanaged);


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
                _confirmationPanel =
                    GetCustomEntityVisualElementComponent<ConfirmationPanelComponent>(
                        BSConst.ConfirmationPanelEntityName);

                if (_stateVisualComponents.Length == 0)
                    Debug.LogWarning("We have a problem with create entities for Building State");

                _bsEcb.AddComponent<ShowVisualElementTag>(_buildingPanel);
                _bsEcb.RemoveComponent<InitializeTag>(entity);
                _bsEcb.SetComponent(_buildingPanel,
                    new BuildingPanelComponent { BuildingPrefabsCount = _stateVisualComponents.Length });

                buildingStateComponent.ValueRW.Self = entity;
                buildingStateComponent.ValueRW.IsInitialized = true;


                BuildingPanelUI.OnBuildSelected += BuildSelected;
                ConfirmationPanelUI.ApplyPanelApplyButton.clicked += ConfirmBuilding;
                ConfirmationPanelUI.ApplyPanelCancelButton.clicked += CancelBuilding;
            }

            if (_gameStateData.ValueRO.CurrentGameState != GameState.BuildingState)
            {
                // Hide panel
                if (_buildingPanel != Entity.Null) _bsEcb.AddComponent<HideVisualElementTag>(_buildingPanel);

                _stateVisualComponents.Dispose();

                BuildingPanelUI.OnBuildSelected -= BuildSelected;
                ConfirmationPanelUI.ApplyPanelApplyButton.clicked -= ConfirmBuilding;
                ConfirmationPanelUI.ApplyPanelCancelButton.clicked -= CancelBuilding;
            }
        }

        private void ConfirmBuilding()
        {
            Debug.Log("confirm  build");
        }

        private void CancelBuilding()
        {
            Debug.Log("cancel build");
            DestroyTempPrefab();
        }

        private void DestroyTempPrefab()
        {
            var s = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempEntity))
            {
                s.AddComponent(tempEntity, new DestroyTempPrefabTag());
                return;
            }

            Debug.LogWarning("We can't find temp build entity!" + this);
        }

        private void BuildSelected(Button button, int index)
        {
            if (_tempSelectedBuildID < 0) // temp not set
            {
                _tempSelectedBuildID = index;
            }
            else if (_tempSelectedBuildID != index) // temp != index
            {
                // destroy temp prefab
                DestroyTempPrefab();
                _tempSelectedBuildID = index;
            }
            else // temp = index
            {
                Debug.LogWarning("We have a problem with enable/disable buttons in BuildPanel." + this);
            }

            var prefabElements = SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsComponent);

            if (!prefabElements.IsEmpty)
            {
                _bsEcb.AddComponent<ShowVisualElementTag>(_confirmationPanel);
                _bsEcb.AddComponent(_gameStateData.ValueRO.BuildingStateEntity,
                    new InstantiateTempPrefabComponent
                    {
                        Prefab = prefabElements[index].PrefabEntity
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

            var nameWithPrefix = BSConst.Prefix + " " + entityName;
            _bsEcb.SetName(entity, nameWithPrefix);

            return entity;
        }
    }
}