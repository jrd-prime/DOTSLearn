using System;
using Jrd.Gameplay.Building.ControlPanel.Component;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.MainStorage.Component;
using Jrd.Gameplay.Storage.ManufacturedBox;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Storage.Warehouse.Component;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.PlayState;
using Jrd.MyUtils;
using Jrd.UI.BuildingControlPanel;
using Jrd.UI.BuildingControlPanel.Part;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.ControlPanel
{
    public partial class BuildingControlPanelSystem : SystemBase
    {
        #region Vars

        private BeginSimulationEntityCommandBufferSystem.Singleton _sys;
        private EntityCommandBuffer _ecb;
        private Entity _buildingEntity;
        private BuildingDataAspect _aspect;
        private MainStorageData _mainStorageData;
        private WarehouseData _warehouseData;
        private InProductionBoxData _inProductionBoxData;
        private ManufacturedBoxData _manufacturedBoxData;
        private BuildingData _buildingData;
        private NativeList<ProductData> _required;
        private NativeList<ProductData> _manufactured;

        private BuildingControlPanelUI _buildingUI;
        private BuildingButtons _buildingButtons;

        #endregion

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<BuildingsPrefabsBufferTag>();
        }

        protected override void OnStartRunning()
        {
            _sys = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            _buildingUI = BuildingControlPanelUI.Instance;
            _buildingButtons = new BuildingButtons();

            _buildingUI.MoveButton.clicked += MoveButton;
            _buildingUI.LoadButton.clicked += LoadButton;
            _buildingUI.TakeButton.clicked += TakeButton;
            _buildingUI.UpgradeButton.clicked += UpgradeButton;
            _buildingUI.BuffButton.clicked += BuffButton;
            _buildingUI.InstantDeliveryButton.clicked += InstantDeliveryButton;
        }

        protected override void OnUpdate()
        {
            _ecb = _sys.CreateCommandBuffer(World.Unmanaged);

            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>().WithAll<SelectedBuildingTag>())
            {
                _aspect = aspect;
                _required = aspect.RequiredProductsData.Required;
                _manufactured = aspect.ManufacturedProductsData.Manufactured;
                _warehouseData = aspect.BuildingProductsData.WarehouseData;
                _inProductionBoxData = aspect.BuildingProductsData.InProductionBoxData;
                _manufacturedBoxData = aspect.BuildingProductsData.ManufacturedBoxData;
                _buildingData = aspect.BuildingData;

                var events = aspect.BuildingData.BuildingEvents;

                if (events.Length > 0) ProcessEvents(events);
            }
        }

        private void ProcessEvents(NativeList<BuildingEvent> events)
        {
            foreach (BuildingEvent ev in events)
            {
                Debug.LogWarning($"BUILDING EVENT: {ev}");
                switch (ev)
                {
                    case BuildingEvent.MoveToWarehouseTimerStarted:
                        OnMoveToWarehouseTimerStarted();
                        break;
                    case BuildingEvent.MoveToWarehouseTimerFinished:
                        OnMoveToWarehouseTimerFinished();
                        break;
                    case BuildingEvent.MoveToProductionBoxFinished:
                        OnMoveToProductionBoxFinished();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                events.RemoveAt(0);
            }
        }


        #region Events Process

        private void OnMoveToWarehouseTimerStarted()
        {
            SetStorageTimer(10, 3); //TODO
            SetItemsToMainStorage();
        }

        private void OnMoveToWarehouseTimerFinished()
        {
            DeliverProductsToWarehouse();
            SetStorageTimer(10, 10); //TODO
            SetItemsToWarehouse();
        }

        private void OnMoveToProductionBoxFinished()
        {
            SetItemsToWarehouse();
            SetItemsToProduction();
        }

        #endregion

        #region Building Timers

        private void UpdateProductionTimers(float all, float one) => _buildingUI.UpdateProductionTimers(all, one);
        private void SetStorageTimer(float max, float value) => _buildingUI.SetTimerText(max, value);

        #endregion

        #region Building Buttons

        public void MoveButton() => _buildingButtons.MoveButton(_aspect.Self, _ecb);
        public void LoadButton() => _buildingButtons.LoadButton(_aspect.Self, _ecb);
        public void TakeButton() => _buildingButtons.TakeButton(_aspect.Self, _ecb);
        public void UpgradeButton() => _buildingButtons.UpgradeButton(_aspect.Self, _ecb);
        public void BuffButton() => _buildingButtons.BuffButton(_aspect.Self, _ecb);
        private void InstantDeliveryButton() => _buildingButtons.InstantDeliveryButton(_aspect.Self, _ecb);

        #endregion

        #region Storage

        private void SetItemsToWarehouse()
        {
            NativeList<ProductData> warehouseProductsList = StorageService.GetProductsDataList(_warehouseData.Value);

            _buildingUI.SetItems(_buildingUI.WarehouseUI, warehouseProductsList);

            warehouseProductsList.Dispose();
        }

        private void SetItemsToInProduction()
        {
            NativeList<ProductData> inProductionData = StorageService.GetProductsDataList(_inProductionBoxData.Value);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionData);

            inProductionData.Dispose();
        }

        private void SetItemsToMainStorage()
        {
            NativeList<ProductData> mainStorageProductsList =
                StorageService.GetMatchingProducts(_required, _mainStorageData.Value, Allocator.Temp);

            _buildingUI.SetItems(_buildingUI.StorageUI, mainStorageProductsList);

            mainStorageProductsList.Dispose();
        }

        public void SetItemsToProduction()
        {
            var inProductionBox =
                Utils.ConvertProductsHashMapToList(_aspect.BuildingProductsData.InProductionBoxData.Value);
            var manufacturedBox = Utils.ConvertProductsHashMapToList(
                _aspect.BuildingProductsData.ManufacturedBoxData.Value);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionBox);
            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedBox);
        }

        private void DeliverProductsToWarehouse()
        {
            var productsToDelivery = SystemAPI.GetComponent<ProductsToDeliveryData>(_aspect.Self).Value;

            _warehouseData.ChangeProductsQuantity(ChangeType.Increase, productsToDelivery);
        }

        #endregion

        #region Main Info

        private void SetMainInfo() => _buildingUI.SetLevel(_buildingData.Level);

        private void SetSpecsInfo()
        {
            // TODO refact
            _buildingUI.SetSpecName(Spec.Productivity, _required.ElementAt(0).Name.ToString());
            _buildingUI.SetSpecName(Spec.LoadCapacity, _required.ElementAt(0).Name.ToString());
            _buildingUI.SetSpecName(Spec.WarehouseCapacity, _manufactured.ElementAt(0).Name.ToString());

            _buildingUI.SetProductivity(_buildingData.ItemsPerHour);
            _buildingUI.SetLoadCapacity(_buildingData.LoadCapacity);
            _buildingUI.SetStorageCapacity(_buildingData.MaxStorage);
        }

        private void SetProductionLineInfo() => _buildingUI.SetLineInfo(_required, _manufactured);

        #endregion
    }
}