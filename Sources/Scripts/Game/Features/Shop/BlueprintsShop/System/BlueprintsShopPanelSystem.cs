using System;
using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.Game.Features.Building.PlaceBuilding;
using Sources.Scripts.UI;
using Sources.Scripts.UI.BlueprintsShopPanel;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UI.PopUpPanels;
using Sources.Scripts.Utility;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.Game.Features.Shop.BlueprintsShop.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial class BlueprintsShopPanelSystem : SystemBase
    {
        #region Private

        private NativeList<Entity> _stateVisualComponents;
        private BeginSimulationEntityCommandBufferSystem.Singleton _bsEcbSystem;
        private BeginInitializationEntityCommandBufferSystem.Singleton _biEcbSystem;
        private EntityCommandBuffer _bsEcb;
        private EntityCommandBuffer _biEcb;

        private int _tempSelectedBuildID;
        private int _prefabsCount;

        private Entity _buildingStateEntity;
        private RefRW<BlueprintsShopData> _buildingPanelData;

        private RefRW<BuildingStateData> _buildingStateData;
        private RefRW<BlueprintsShopData> _blueprintsShopData;
        private DynamicBuffer<BlueprintsBuffer> _blueprintsBuffers;
        private int _buildingsCount;

        private bool _isShopSelected;

        private BlueprintsShopPanelUI _blueprintsShopPanelUI;

        #endregion

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<BlueprintsBuffer>();
        }

        protected override void OnStartRunning()
        {
            _tempSelectedBuildID = -1;
            _isShopSelected = false;
            _blueprintsShopPanelUI = BlueprintsShopPanelUI.Instance;

            _blueprintsShopPanelUI.PanelCloseButton.clicked += OnBlueprintsShopClosed;

            MainUIButtonsMono.BlueprintsShopButton.clicked += BlueprintsShopSelected;
            
            BlueprintsShopPanelUI.OnBlueprintSelected += BlueprintSelected;
            
            ConfirmationPanelUI.OnTempBuildCancelled += CancelBuilding;
            ConfirmationPanelUI.OnTempBuildApply += ConfirmBuilding;
            
            BuildingControlPanelUI.Instance.PanelCloseButton.clicked += ClosePanelAndRemoveSelectedTag;
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<BlueprintsBuffer> buffer))
            {
                Debug.LogError("Buffer error. Return.. " + this);
                return;
            }

            if (!SystemAPI.TryGetSingletonRW<BlueprintsShopData>(out var blueprintsShopData))
            {
                Debug.Log(" NOUUU BlueprintsShopData");
                return;
            }

            if (!SystemAPI.TryGetSingletonRW<BuildingStateData>(out var buildingStateData))
            {
                Debug.Log(" NOUUU BuildingStateData");
                return;
            }

            _blueprintsShopData = blueprintsShopData;
            _buildingStateData = buildingStateData;

            _buildingsCount = _buildingStateData.ValueRO.BuildingPrefabsCount;

            _blueprintsBuffers = buffer;

            _prefabsCount = buffer.Length;

            _biEcbSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            _bsEcbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _bsEcbSystem.CreateCommandBuffer(World.Unmanaged);

            _buildingStateEntity = SystemAPI.GetSingletonEntity<BuildingStateData>();
            _buildingPanelData = SystemAPI.GetSingletonRW<BlueprintsShopData>();

            if (!_buildingStateData.ValueRO.IsInitialized) Initialize();
        }

        private void BlueprintsShopSelected()
        {
            if (!_isShopSelected)
            {
                OnBlueprintsShopOpened();
            }
            else
            {
                OnBlueprintsShopClosed();
            }
        }

        private void OnBlueprintsShopClosed()
        {
            _blueprintsShopPanelUI.SetElementVisible(false);
            _isShopSelected = false;
        }

        private void OnBlueprintsShopOpened()
        {
            _blueprintsShopPanelUI.InstantiateBuildingsCards(_buildingsCount, GetNamesList());
            _blueprintsShopPanelUI.SetPanelTitle("Panel Title New");
            SetBlueprintsShopPanelVisible(true);

            _isShopSelected = true;
        }

        private void SetBlueprintsShopPanelVisible(bool value) =>
            _blueprintsShopPanelUI.SetElementVisible(value);

        private void SetConfirmationPanelVisible(bool value) => ConfirmationPanelUI.Instance.SetElementVisible(value);

        private void BlueprintSelected(Button button, int index)
        {
            OnBlueprintsShopClosed();
            SetConfirmationPanelVisible(true);
            SetTempSelectedBuildingId(index);
            InstantiateTempPrefab(index);
        }

        private void ClosePanelAndRemoveSelectedTag()
        {
            Debug.LogWarning("ClosePanelAndRemoveSelectedTag ???");
            BuildingControlPanelUI.Instance.SetElementVisible(false);

            var e = SystemAPI.GetSingletonEntity<SelectedBuildingTag>();
            _bsEcb.RemoveComponent<SelectedBuildingTag>(e);
        }

        private void InstantiateTempPrefab(int index)
        {
            if (_blueprintsBuffers.IsEmpty) throw new NullReferenceException("Buffer empty!");
            

            BlueprintsBuffer blueprintBuffer = _blueprintsBuffers[index];

            FixedString64Bytes giud = Utils.GetGuid();

            _bsEcb.AddComponent(_buildingStateEntity,
                new InstantiateTempBuildingData
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

            Debug.Log($"Build ID: {index} / Prefab: {_blueprintsBuffers[index].Name}");
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

            OnBlueprintsShopOpened();

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
                _biEcb.AddComponent(tempEntity, new DestroyTempBuildingTag());
            }
        }

        protected override void OnDestroy()
        {
            MainUIButtonsMono.BlueprintsShopButton.clicked -= BlueprintsShopSelected;
            BlueprintsShopPanelUI.OnBlueprintSelected -= BlueprintSelected;
            ConfirmationPanelUI.OnTempBuildCancelled -= CancelBuilding;
            ConfirmationPanelUI.OnTempBuildApply -= ConfirmBuilding;
        }

        private void Initialize()
        {
            _buildingStateData.ValueRW.BuildingPrefabsCount = _prefabsCount;
            _buildingStateData.ValueRW.IsInitialized = true;
        }

        private NativeList<FixedString32Bytes> GetNamesList()
        {
            NativeList<FixedString32Bytes> namesList = new(_buildingsCount, Allocator.Temp);

            foreach (var building in _blueprintsBuffers)
            {
                namesList.Add(new FixedString32Bytes(building.Name));
            }

            return namesList;
        }
    }
}