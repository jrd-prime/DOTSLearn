using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Jrd.GameStates.SharedComponentAndSystems;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    [UpdateAfter(typeof(MainGameState.GameStatesSystem))]
    public partial class BuildingStateSystem : SystemBase
    {
        private EntityManager _em;
        private NativeList<Entity> _stateVisualComponents;
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _eiEcb;

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
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            if (!_stateVisualComponents.IsCreated)
                _stateVisualComponents = new NativeList<Entity>(2, Allocator.Persistent); // TODO подумать

            _gameStateEntity = SystemAPI.GetSingletonEntity<GameStateData>();
            _gameStateData = SystemAPI.GetComponentRW<GameStateData>(_gameStateEntity); // TODO aspect

            _eiEcb = _ecbSystem.CreateCommandBuffer(World.Unmanaged);


            // Init by tag // LOOK TODO вытащить в отдельную систему обобщенную
            foreach (var (buildingStateComponent, entity) in SystemAPI
                         .Query<RefRW<BuildingStateComponent>>()
                         .WithAll<InitializeTag>()
                         .WithEntityAccess())
            {
                Debug.Log("init building state system");
                if (buildingStateComponent.ValueRO.IsInitialized) return;

                _eiEcb.RemoveComponent<InitializeTag>(entity);

                _buildingPanel =
                    GetCustomEntityWithVisualElementComponent<BuildingPanelComponent>(BSConst.BuildingPanelEntityName);
                _confirmationPanel =
                    GetCustomEntityWithVisualElementComponent<ConfirmationPanelComponent>(
                        BSConst.ConfirmationPanelEntityName);

                if (_stateVisualComponents.Length == 0)
                    Debug.LogWarning("We have a problem with create entities for Building State");

                _eiEcb.AddComponent<ShowVisualElementTag>(_buildingPanel);


                _eiEcb.SetComponent(_buildingPanel,
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
                if (_buildingPanel != Entity.Null) _eiEcb.AddComponent<HideVisualElementTag>(_buildingPanel);

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

            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempEntity))
            {
                _eiEcb.AddComponent(tempEntity, new DestroyTempPrefabTag { });
            }
            else
            {
                Debug.LogWarning("We can't find temp build entity!");
            }
        }

        private void BuildSelected(Button button, int index)
        {
            if (_tempSelectedBuildID < 0)
            {
                // temp not set
                BuildingPanelUI.DisableButton(index);
                _tempSelectedBuildID = index;
            }
            else if (_tempSelectedBuildID != index)
            {
                // temp != index
                BuildingPanelUI.DisableButton(index);
                BuildingPanelUI.EnableButton(_tempSelectedBuildID);
                _tempSelectedBuildID = index;
            }
            else
            {
                // temp = index
                Debug.LogWarning("We have a problem with enable/disable buttons in BuildPanel.");
            }


            _eiEcb = _ecbSystem.CreateCommandBuffer(World.Unmanaged);
            _eiEcb.AddComponent<ShowVisualElementTag>(_confirmationPanel);

            var prefabElements = SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsComponent);

            if (prefabElements.IsEmpty)
            {
                Debug.LogWarning("Prefabs: " + prefabElements.Length);
                return;
            }

            _eiEcb.AddComponent(_gameStateData.ValueRO.BuildingStateEntity, new InstantiateTempPrefabComponent
            {
                Prefab = prefabElements[index].PrefabEntity
            });


            Debug.Log($"Build Selected. ID: {index} / Btn: {button.name} / Prefab: {prefabElements[index].PrefabName}");
        }

        private Entity GetCustomEntityWithVisualElementComponent<T>(FixedString64Bytes entityName)
            where T : unmanaged, IComponentData
        {
            var entity = _em.CreateEntity(); // TODO
            _stateVisualComponents.Add(entity);

            _eiEcb.AddComponent<T>(entity);
            _eiEcb.AddComponent<VisibilityComponent>(entity);
            _eiEcb.SetComponent(entity, new VisibilityComponent { IsVisible = false });

            var nameWithPrefix = BSConst.Prefix + " " + entityName;
            _eiEcb.SetName(entity, nameWithPrefix);

            Debug.Log("new entity " + nameWithPrefix + " / " + entity);

            return entity;
        }
    }
}