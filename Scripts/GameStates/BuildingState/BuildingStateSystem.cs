using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    [UpdateAfter(typeof(GameStatesSystem))]
    public partial class BuildingStateSystem : SystemBase
    {
        private RefRW<BuildingPanelData> _a;
        private EntityManager _entityManager;
        private NativeList<Entity> _stateVisualComponents;
        private BeginSimulationEntityCommandBufferSystem.Singleton _bsEcbSystem;
        private BeginInitializationEntityCommandBufferSystem.Singleton _biEcbSystem;
        private EntityCommandBuffer _bsEcb;
        private EntityCommandBuffer _biEcb;

        private Entity _confirmationPanel;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;
        private int _tempSelectedBuildID;
        private DynamicBuffer<BuildingsPrefabsBuffer> _buildingsPrefabsBuffer;

        private RefRW<BuildingStateData> _buildingStateData;
        private RefRW<BuildingPanelData> _buildingPanelData;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<GameStateData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _entityManager = EntityManager;
        }

        protected override void OnStartRunning()
        {
            _tempSelectedBuildID = -1;
            MainUIButtonsMono.BuildingStateButton.clicked += OnBuildingStateSelected;

            // UI_old.BuildingPanelUI.OnBuildSelected += BuildSelected;
            // ConfirmationPanelUI.ApplyPanelApplyButton.clicked += ConfirmBuilding;
            // ConfirmationPanelUI.ApplyPanelCancelButton.clicked += CancelBuilding;
        }

        protected override void OnStopRunning()
        {
            MainUIButtonsMono.BuildingStateButton.clicked -= OnBuildingStateSelected;
        }

        private void OnBuildingStateSelected()
        {
            _buildingPanelData = SystemAPI.GetSingletonRW<BuildingPanelData>();
            // Building Panel
            _buildingPanelData.ValueRW.SetVisible = !_buildingPanelData.ValueRO.SetVisible;
        }

        protected override void OnUpdate()
        {
            // LOOK wait load
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BuildingsPrefabsBuffer> buffer))
            {
                Debug.LogError("Buffer error. Return.. " + this);
                return;
            }

            _buildingsPrefabsBuffer = buffer;

            {
                _biEcbSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
                _bsEcbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                _bsEcb = _bsEcbSystem.CreateCommandBuffer(World.Unmanaged);
            }

            _gameStateData = SystemAPI.GetSingletonRW<GameStateData>();

            _buildingStateData = SystemAPI.GetSingletonRW<BuildingStateData>();
            _buildingPanelData = SystemAPI.GetSingletonRW<BuildingPanelData>();


            if (!_buildingStateData.ValueRO.IsInitialized) Initialize();
        }

        private void Initialize()
        {
            Debug.Log("Initialize Building State Data");

            int buildingsPrefabsCount = _buildingsPrefabsBuffer.Length;

            _buildingStateData.ValueRW.BuildingPrefabsCount = buildingsPrefabsCount;
            _buildingStateData.ValueRW.IsInitialized = true;
        }

        private void ConfirmBuilding()
        {
            Debug.Log("confirm  build");

            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempBuildingEntity))
            {
                _bsEcb.AddComponent<PlaceTempBuildingTag>(tempBuildingEntity);

                UI_old.BuildingPanelUI.SetButtonEnabled(_tempSelectedBuildID, true);
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

            if (!_buildingsPrefabsBuffer.IsEmpty)
            {
                _bsEcb.AddComponent<ShowVisualElementTag>(_confirmationPanel);
                _bsEcb.AddComponent(_gameStateData.ValueRO.BuildingStateEntity,
                    new InstantiateTempPrefabComponent
                    {
                        Prefab = _buildingsPrefabsBuffer[index].PrefabEntity,
                        Name = _buildingsPrefabsBuffer[index].PrefabName
                    });

                Debug.Log(
                    $"Build Selected. ID: {index} / Btn: {button.name} / Prefab: {_buildingsPrefabsBuffer[index].PrefabName}");
                return;
            }

            Debug.LogError("Prefabs: " + _buildingsPrefabsBuffer.Length);
        }
    }
}