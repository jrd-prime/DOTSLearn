using System;
using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.Events;
using Sources.Scripts.Game.Features.Building.Storage;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage;
using Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox;
using Sources.Scripts.Game.Features.Building.Storage.Service;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UI.BuildingControlPanel.Part;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
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
                _warehouseData = aspect.ProductsInBuildingData.WarehouseData;
                _inProductionBoxData = aspect.ProductsInBuildingData.InProductionBoxData;
                _manufacturedBoxData = aspect.ProductsInBuildingData.ManufacturedBoxData;
                _buildingData = aspect.BuildingData;

                ProcessEvents(aspect.BuildingData.BuildingEvents);
            }
        }

        private void ProcessEvents(NativeQueue<BuildingEvent> eQueue)
        {
            while (eQueue.Count > 0)
            {
                var ev = eQueue.Dequeue();

                Debug.LogWarning($"___ BUILDING EVENT: {ev}");
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
                    case BuildingEvent.MainStorageDataUpdated:
                        SetItemsToMainStorage();
                        break;
                    case BuildingEvent.WarehouseDataUpdated:
                        SetItemsToWarehouse();
                        break;
                    case BuildingEvent.InProductionBoxDataUpdated:
                        SetItemsToInProductionBox();
                        break;
                    case BuildingEvent.ManufacturedBoxDataUpdated:
                        SetItemsToManufacturedBox();
                        break;
                    case BuildingEvent.OneLoadCycleFinished:
                        _aspect.ProductionProcessData.ValueRW.CurrentCycle += 1;

                        var currentCycle = _aspect.ProductionProcessData.ValueRO.CurrentCycle;
                        var maxLoads = _aspect.ProductionProcessData.ValueRO.MaxLoads;

                        UpdateProductionTimers(currentCycle, maxLoads);

                        break;
                    case BuildingEvent.FullLoadCycleFinished:
                        _aspect.ProductionProcessData.ValueRW.LastCycleEnd = true;
                        _buildingUI.LoadButton.SetEnabled(true);
                        break;

                    case BuildingEvent.ProductionTimersStarted:
                        OnProductionTimersStarted();
                        break;
                    case BuildingEvent.ProductionTimersInProgressUpdate:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnProductionTimersStarted()
        {
            var currentCycle = _aspect.ProductionProcessData.ValueRO.CurrentCycle;
            var maxLoads = _aspect.ProductionProcessData.ValueRO.MaxLoads;

            UpdateProductionTimers(currentCycle, maxLoads);

            _aspect.ProductionProcessData.ValueRW.LastCycleEnd = false;
            _buildingUI.LoadButton.SetEnabled(false);
        }


        private void SetItemsToManufacturedBox()
        {
            NativeList<ProductData> manufacturedProductsList =
                StorageService.GetProductsDataList(_manufacturedBoxData.Value);

            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedProductsList);

            manufacturedProductsList.Dispose();
        }

        #region Events Process

        private void OnMoveToWarehouseTimerStarted()
        {
            SetStorageTimer(10, 3); //TODO
            SetItemsToMainStorage();
            _buildingUI.MoveButton.SetEnabled(false);
        }

        private void OnMoveToWarehouseTimerFinished()
        {
            DeliverProductsToWarehouse();
            SetStorageTimer(10, 10); //TODO
            SetItemsToWarehouse();
            _buildingUI.MoveButton.SetEnabled(true);
        }

        private void OnMoveToProductionBoxFinished()
        {
            SetItemsToWarehouse();
            SetItemsToProduction();
        }

        #endregion

        #region Building Timers

        private void UpdateProductionTimers(int currentCycle, int maxLoads) =>
            _buildingUI.UpdateProductionTimers(currentCycle, maxLoads);

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

        private void SetItemsToInProductionBox()
        {
            NativeList<ProductData> inProductionData = StorageService.GetProductsDataList(_inProductionBoxData.Value);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionData);

            inProductionData.Dispose();
        }

        private void SetItemsToMainStorage()
        {
            NativeList<ProductData> mainStorageProductsList =
                StorageService.GetMatchingProducts(_required, _mainStorageData.Value, out bool isEnough);
            Debug.LogWarning("SetItemsToMainStorage()");
            foreach (var q in mainStorageProductsList)
            {
                Debug.LogWarning($"set? {q.Name} / {q.Quantity}");
            }

            _buildingUI.SetItems(_buildingUI.StorageUI, mainStorageProductsList);

            mainStorageProductsList.Dispose();
        }

        public void SetItemsToProduction()
        {
            ProductData.ConvertProductsHashMapToList(
                _aspect.ProductsInBuildingData.InProductionBoxData.Value,
                out NativeList<ProductData> inProductionBox);

            ProductData.ConvertProductsHashMapToList(
                _aspect.ProductsInBuildingData.ManufacturedBoxData.Value,
                out NativeList<ProductData> manufacturedBox);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionBox);
            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedBox);
        }

        private void DeliverProductsToWarehouse()
        {
            var productsToDelivery = SystemAPI.GetComponent<ProductsToDeliveryData>(_aspect.Self).Value;
            _aspect.ChangeProductsQuantityData.Value.Enqueue(new ChangeProductsQuantityData
            {
                StorageType = StorageType.Warehouse,
                ChangeType = ChangeType.Increase,
                ProductsData = productsToDelivery
            });
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