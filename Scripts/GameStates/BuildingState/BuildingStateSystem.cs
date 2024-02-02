using System;
using Jrd.GameplayBuildings;
using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.BuildingState.TempBuilding;
using Jrd.GameStates.MainGameState;
using Jrd.GameStates.PlayState;
using Jrd.UI;
using Jrd.UI.BuildingInfoPanel;
using Jrd.UI.BuildingState;
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

        private RefRW<ConfirmationPanelData> _confirmationPanelData;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;
        private int _tempSelectedBuildID;
        private int _prefabsCount;
        private DynamicBuffer<BuildingsBuffer> _buildingsPrefabsBuffer;

        private Entity _buildingStateEntity;
        private RefRW<BuildingStateData> _buildingStateData;
        private RefRW<BuildingPanelData> _buildingPanelData;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<GameStateData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        protected override void OnStartRunning()
        {
            _entityManager = EntityManager;
            _tempSelectedBuildID = -1;

            MainUIButtonsMono.BuildingStateButton.clicked += BuildingStateSelected;
            BuildingPanelMono.OnBuildSelected += BuildSelected;
            ConfirmationPanelMono.OnTempBuildCancelled += CancelBuilding;
            ConfirmationPanelMono.OnTempBuildApply += ConfirmBuilding;
            BuildingInfoPanelUIController.Instance.PanelCloseButton.clicked += ClosePanelAndRemoveSelectedTag;

            // ConfirmationPanelUI.ApplyPanelApplyButton.clicked += ConfirmBuilding;
        }

        private void ClosePanelAndRemoveSelectedTag()
        {
            BuildingInfoPanelUIController.Instance.SetElementVisible(false);

            var e = SystemAPI.GetSingletonEntity<SelectedBuildingTag>();
            _bsEcb.RemoveComponent<SelectedBuildingTag>(e);
        }

        protected override void OnUpdate()
        {
            // LOOK wait load
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BuildingsBuffer> buffer))
            {
                Debug.LogError("Buffer error. Return.. " + this);
                return;
            }

            _buildingsPrefabsBuffer = buffer;
            _prefabsCount = buffer.Length;

            _biEcbSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            _bsEcbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _bsEcbSystem.CreateCommandBuffer(World.Unmanaged);

            _gameStateData = SystemAPI.GetSingletonRW<GameStateData>();

            _buildingStateEntity = SystemAPI.GetSingletonEntity<BuildingStateData>();
            _buildingStateData = SystemAPI.GetSingletonRW<BuildingStateData>();
            _buildingPanelData = SystemAPI.GetSingletonRW<BuildingPanelData>();
            _confirmationPanelData = SystemAPI.GetSingletonRW<ConfirmationPanelData>();

            if (!_buildingStateData.ValueRO.IsInitialized) Initialize();
        }

        private void BuildingStateSelected()
        {
            SetBuildingPanelVisible(true);
        }

        private void SetBuildingPanelVisible(bool value) => _buildingPanelData.ValueRW.SetVisible = value;
        private void SetConfirmationPanelVisible(bool value) => _confirmationPanelData.ValueRW.SetVisible = value;


        private void BuildSelected(Button button, int index)
        {
            SetBuildingPanelVisible(false);
            SetConfirmationPanelVisible(true);
            SetTempSelectedBuildingId(index);
            InstantiateTempPrefab(index);
        }

        private void InstantiateTempPrefab(int index)
        {
            if (_buildingsPrefabsBuffer.IsEmpty)
            {
                throw new NullReferenceException("Buffer empty!");
            }

            BuildingsBuffer buildingBuffer = _buildingsPrefabsBuffer[index];

            FixedString64Bytes giud = Utils.Utils.GetGuid();

            _bsEcb.AddComponent(_buildingStateEntity,
                new InstantiateTempPrefabComponent
                {
                    BuildingData = new BuildingData
                    {
                        Guid = giud,
                        Name = buildingBuffer.Name,
                        Prefab = buildingBuffer.Self,

                        NameId = buildingBuffer.NameId,
                        Level = buildingBuffer.Level,
                        ItemsPerHour = buildingBuffer.ItemsPerHour,
                        LoadCapacity = buildingBuffer.LoadCapacity,
                        MaxStorage = buildingBuffer.StorageCapacity
                    }
                });

            Debug.Log($"Build ID: {index} / Prefab: {_buildingsPrefabsBuffer[index].Name}");
        }

        private void SetTempSelectedBuildingId(int index)
        {
            if (_tempSelectedBuildID < 0)
            {
                // SetButtonEnabled(index, false);
                _tempSelectedBuildID = index;
            }
            else if (_tempSelectedBuildID != index)
            {
                // SetButtonEnabled(index, false);
                // SetButtonEnabled(_tempSelectedBuildID, true);
                _tempSelectedBuildID = index;
            }
            else
            {
                Debug.LogWarning("Temp = Index! / We have a problem with enable/disable buttons in " + this);
            }
        }

        private void CancelBuilding()
        {
            Debug.Log("cancel build");
            SetConfirmationPanelVisible(false);
            DestroyTempPrefab();

            // reset temp id
            _tempSelectedBuildID = -1;
        }

        private void ConfirmBuilding()
        {
            Debug.Log("confirm  build");

            SetConfirmationPanelVisible(false);
            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempBuildingEntity))
            {
                _bsEcb.AddComponent<PlaceTempBuildingTag>(tempBuildingEntity);

                // UI_old.BuildingPanelUI.SetButtonEnabled(_tempSelectedBuildID, true);
                _tempSelectedBuildID = -1;
            }
            else
            {
                Debug.LogWarning("We can't find temp building entity!");
            }
        }

        private void DestroyTempPrefab()
        {
            _biEcb = _biEcbSystem.CreateCommandBuffer(World.Unmanaged);

            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempEntity))
            {
                _biEcb.AddComponent(tempEntity, new DestroyTempPrefabTag());
            }
        }

        protected override void OnStopRunning()
        {
            MainUIButtonsMono.BuildingStateButton.clicked -= BuildingStateSelected;
            BuildingPanelMono.OnBuildSelected -= BuildSelected;
            ConfirmationPanelMono.OnTempBuildCancelled -= CancelBuilding;
            ConfirmationPanelMono.OnTempBuildApply -= ConfirmBuilding;
        }

        private void Initialize()
        {
            _buildingStateData.ValueRW.BuildingPrefabsCount = _prefabsCount;
            _buildingStateData.ValueRW.IsInitialized = true;
        }
    }
}