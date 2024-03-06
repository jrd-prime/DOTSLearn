using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage.Data;
using Sources.Scripts.CommonData.Storage.Service;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UI.BuildingControlPanel.Part;
using Unity.Collections;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Panel
{
    public class BuildingUIUpdater
    {
        #region Private

        private BuildingDataAspect _aspect;
        private BuildingData _mainData;
        private NativeList<ProductData> _productsToDelivery;

        private NativeList<ProductData> _required;
        private NativeList<ProductData> _manufactured;

        private NativeParallelHashMap<int, int> _mainStorageData;
        private NativeParallelHashMap<int, int> _warehouseData;
        private NativeParallelHashMap<int, int> _inProductionBox;
        private NativeParallelHashMap<int, int> _manufacturedBox;

        private readonly BuildingControlPanelUI _buildingUI;

        #endregion

        public BuildingUIUpdater()
        {
            _buildingUI = BuildingControlPanelUI.Instance;
        }

        public void UpdateVarsData(ref EventsDataWrapper eventsDataWrapper)
        {
            _aspect = eventsDataWrapper.Aspect;
            _mainData = _aspect.BuildingData;
            _productsToDelivery = eventsDataWrapper.ProductsToDelivery;

            _required = _aspect.RequiredProductsData.Value;
            _manufactured = _aspect.ManufacturedProductsData.Value;

            _mainStorageData = eventsDataWrapper.MainStorageBoxData.Value;
            _warehouseData = _aspect.ProductsInBuildingData.WarehouseBoxData.Value;
            _inProductionBox = _aspect.ProductsInBuildingData.InProductionBoxData.Value;
            _manufacturedBox = _aspect.ProductsInBuildingData.ManufacturedBoxData.Value;
        }


        #region Timers

        public void UpdateProductionTimers(int getLoadedProductsManufacturingTime)
        {
            var currentCycle = _aspect.ProductionProcessData.ValueRO.CurrentCycle;
            var maxLoads = _aspect.ProductionProcessData.ValueRO.MaxLoads;

            _buildingUI.UpdateProductionTimers(currentCycle, maxLoads, getLoadedProductsManufacturingTime);
        }

        public void RunMoveFromStorageTimerAsync(float duration) => _buildingUI.RunMoveFromStorageTimerAsync(duration);

        #endregion

        #region Storage

        public void SetItemsToMainStorage()
        {
            NativeList<ProductData> prods =
                StorageService.GetMatchingProducts(_required, _mainStorageData, out bool isEnough);

            _buildingUI.SetItems(_buildingUI.StorageUI, prods);

            prods.Dispose();
        }

        public void SetItemsToWarehouse()
        {
            NativeList<ProductData> prods = StorageService.GetProductsDataList(_warehouseData);

            _buildingUI.SetItems(_buildingUI.WarehouseUI, prods);

            prods.Dispose();
        }

        public void SetItemsToInProductionBox()
        {
            ProductData.ConvertProductsHashMapToList(_inProductionBox, out NativeList<ProductData> data);

            _buildingUI.SetItems(_buildingUI.InProductionUI, data);

            data.Dispose();
        }
        
        public void SetItemsToManufacturedBox()
        {
            ProductData.ConvertProductsHashMapToList(_manufacturedBox, out NativeList<ProductData> data);

            _buildingUI.SetItems(_buildingUI.ManufacturedUI, data);

            data.Dispose();
        }

        public void SetItemsToProduction()
        {
            ProductData.ConvertProductsHashMapToList(_inProductionBox, out NativeList<ProductData> inProductionBox);
            ProductData.ConvertProductsHashMapToList(_manufacturedBox, out NativeList<ProductData> manufacturedBox);

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