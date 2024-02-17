using System;
using Jrd.Gameplay.Building;
using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Building.ControlPanel.Component;
using Jrd.Gameplay.Building.TempBuilding;
using Jrd.Gameplay.Building.TempBuilding.Component;
using Jrd.Gameplay.Shop.BlueprintsShop;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.MainGameState;
using Jrd.GameStates.PlayState;
using Jrd.MyUtils;
using Jrd.UI;
using Jrd.UI.BlueprintsShopPanel;
using Jrd.UI.BuildingControlPanel;
using Jrd.UI.PopUpPanels;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    [UpdateAfter(typeof(GameStatesSystem))]
    public partial class BuildingStateSystem : SystemBase
    {
        private RefRW<BlueprintsShopData> _a;
        private EntityManager _entityManager;
        private NativeList<Entity> _stateVisualComponents;
        private BeginSimulationEntityCommandBufferSystem.Singleton _bsEcbSystem;
        private BeginInitializationEntityCommandBufferSystem.Singleton _biEcbSystem;
        private EntityCommandBuffer _bsEcb;
        private EntityCommandBuffer _biEcb;

        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;
        private int _tempSelectedBuildID;
        private int _prefabsCount;
        private DynamicBuffer<BlueprintsBuffer> _buildingsPrefabsBuffer;

        private Entity _buildingStateEntity;
        private RefRW<BuildingStateData> _buildingStateData;
        private RefRW<BlueprintsShopData> _buildingPanelData;

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

            MainUIButtonsMono.BlueprintsShopButton.clicked += BlueprintsShopSelected;
            BlueprintsShopPanelUI.OnBuildSelected += BuildSelected;
            ConfirmationPanelUI.OnTempBuildCancelled += CancelBuilding;
            ConfirmationPanelUI.OnTempBuildApply += ConfirmBuilding;
            BuildingControlPanelUI.Instance.PanelCloseButton.clicked += ClosePanelAndRemoveSelectedTag;

            // ConfirmationPanelUI.ApplyPanelApplyButton.clicked += ConfirmBuilding;
        }

        private void ClosePanelAndRemoveSelectedTag()
        {
            BuildingControlPanelUI.Instance.SetElementVisible(false);

            var e = SystemAPI.GetSingletonEntity<SelectedBuildingTag>();
            _bsEcb.RemoveComponent<SelectedBuildingTag>(e);
        }

        protected override void OnUpdate()
        {
            // LOOK wait load
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BlueprintsBuffer> buffer))
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
            _buildingPanelData = SystemAPI.GetSingletonRW<BlueprintsShopData>();

            if (!_buildingStateData.ValueRO.IsInitialized) Initialize();
        }

        private void BlueprintsShopSelected()
        {
            SetBlueprintsShopPanelVisible(true);
        }

        private void SetBlueprintsShopPanelVisible(bool value) => _buildingPanelData.ValueRW.SetVisible = value;

        // private void SetConfirmationPanelVisible(bool value) => _confirmationPanelData.ValueRW.SetVisible = value;
        private void SetConfirmationPanelVisible(bool value) => ConfirmationPanelUI.Instance.SetElementVisible(value);


        private void BuildSelected(Button button, int index)
        {
            SetBlueprintsShopPanelVisible(false);
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

            BlueprintsBuffer blueprintBuffer = _buildingsPrefabsBuffer[index];

            FixedString64Bytes giud = Utils.GetGuid();

            _bsEcb.AddComponent(_buildingStateEntity,
                new InstantiateTempPrefabComponent
                {
                    BuildingData = new BuildingData
                    {
                        Guid = giud,
                        Name = blueprintBuffer.Name,
                        Prefab = blueprintBuffer.Self,
                        BuildingEvents = new NativeQueue<BuildingEvent>(Allocator.Persistent),

                        NameId = blueprintBuffer.NameId,
                        Level = blueprintBuffer.Level,
                        ItemsPerHour = blueprintBuffer.ItemsPerHour,
                        LoadCapacity = blueprintBuffer.LoadCapacity,
                        MaxStorage = blueprintBuffer.StorageCapacity
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
            SetConfirmationPanelVisible(false);
            DestroyTempPrefab();

            // reset temp id
            _tempSelectedBuildID = -1;
        }

        private void ConfirmBuilding()
        {
            SetConfirmationPanelVisible(false);
            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempBuildingEntity))
            {
                _bsEcb.AddComponent<PlaceTempBuildingTag>(tempBuildingEntity);
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
            MainUIButtonsMono.BlueprintsShopButton.clicked -= BlueprintsShopSelected;
            BlueprintsShopPanelUI.OnBuildSelected -= BuildSelected;
            ConfirmationPanelUI.OnTempBuildCancelled -= CancelBuilding;
            ConfirmationPanelUI.OnTempBuildApply -= ConfirmBuilding;
        }

        private void Initialize()
        {
            _buildingStateData.ValueRW.BuildingPrefabsCount = _prefabsCount;
            _buildingStateData.ValueRW.IsInitialized = true;
        }
    }
}