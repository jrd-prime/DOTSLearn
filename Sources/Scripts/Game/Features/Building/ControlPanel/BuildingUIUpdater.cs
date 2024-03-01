using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Sources.Scripts.Game.Features.Building.Storage.Service;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UI.BuildingControlPanel.Part;
using Unity.Collections;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.ControlPanel
{
    public struct BuildingUIUpdater
    {
        private readonly BuildingControlPanelUI _buildingUI;
        private readonly BuildingDataAspect _aspect;

        private readonly BuildingData _mainData;

        private readonly NativeParallelHashMap<int, int> _mainStorageData;
        private readonly NativeParallelHashMap<int, int> _warehouseData;
        private readonly NativeParallelHashMap<int, int> _inProductionBoxData;
        private readonly NativeParallelHashMap<int, int> _manufacturedBoxData;

        private NativeList<ProductData> _required;
        private readonly NativeList<ProductData> _manufactured;

        private readonly NativeList<ProductData> _productsToDelivery;

        public BuildingUIUpdater(BuildingControlPanelUI buildingControlPanelUI,
            BuildingDataAspect aspect,
            NativeParallelHashMap<int, int> mainStorageData,
            NativeList<ProductData> productsToDelivery)
        {
            _buildingUI = buildingControlPanelUI;
            _aspect = aspect;

            _mainData = aspect.BuildingData;

            _required = aspect.RequiredProductsData.Value;
            _manufactured = aspect.ManufacturedProductsData.Value;

            _mainStorageData = mainStorageData;
            _warehouseData = aspect.ProductsInBuildingData.WarehouseData.Value;
            _inProductionBoxData = aspect.ProductsInBuildingData.InProductionBoxData.Value;
            _manufacturedBoxData = aspect.ProductsInBuildingData.ManufacturedBoxData.Value;

            _productsToDelivery = productsToDelivery;
        }


        public void SetItemsToManufacturedBox()
        {
            NativeList<ProductData> manufacturedProductsList =
                StorageService.GetProductsDataList(_manufacturedBoxData);

            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedProductsList);

            manufacturedProductsList.Dispose();
        }


        #region Building Timers

        public void UpdateProductionTimers(int currentCycle, int maxLoads) =>
            _buildingUI.UpdateProductionTimers(currentCycle, maxLoads);

        public void SetStorageTimer(float max, float value) => _buildingUI.SetTimerText(max, value);

        #endregion

        #region Storage

        public void SetItemsToWarehouse()
        {
            NativeList<ProductData> warehouseProductsList = StorageService.GetProductsDataList(_warehouseData);

            _buildingUI.SetItems(_buildingUI.WarehouseUI, warehouseProductsList);

            warehouseProductsList.Dispose();
        }

        public void SetItemsToInProductionBox()
        {
            NativeList<ProductData> inProductionData = StorageService.GetProductsDataList(_inProductionBoxData);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionData);

            inProductionData.Dispose();
        }

        public void SetItemsToMainStorage()
        {
            NativeList<ProductData> mainStorageProductsList =
                StorageService.GetMatchingProducts(_required, _mainStorageData, out bool isEnough);
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
            ProductData.ConvertProductsHashMapToList(_inProductionBoxData, out NativeList<ProductData> inProductionBox);
            ProductData.ConvertProductsHashMapToList(_manufacturedBoxData, out NativeList<ProductData> manufacturedBox);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionBox);
            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedBox);
        }

        public void DeliverProductsToWarehouse()
        {
            _aspect.ChangeProductsQuantityData.Value.Enqueue(new ChangeProductsQuantityData
            {
                StorageType = StorageType.Warehouse,
                ChangeType = ChangeType.Increase,
                ProductsData = _productsToDelivery
            });
        }

        #endregion

        #region Main Info

        public void SetMainInfo() => _buildingUI.SetLevel(_aspect.BuildingData.Level);

        public void SetSpecsInfo()
        {
            // TODO refact
            _buildingUI.SetSpecName(Spec.Productivity, _required.ElementAt(0).Name.ToString());
            _buildingUI.SetSpecName(Spec.LoadCapacity, _required.ElementAt(0).Name.ToString());
            _buildingUI.SetSpecName(Spec.WarehouseCapacity,
                _aspect.ManufacturedProductsData.Value.ElementAt(0).Name.ToString());

            _buildingUI.SetProductivity(_mainData.ItemsPerHour);
            _buildingUI.SetLoadCapacity(_mainData.LoadCapacity);
            _buildingUI.SetStorageCapacity(_mainData.MaxStorage);
        }

        public void SetProductionLineInfo() => _buildingUI.SetLineInfo(_required, _manufactured);

        #endregion
    }
}