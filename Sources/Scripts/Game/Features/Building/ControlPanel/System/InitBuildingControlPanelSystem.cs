using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage.Data;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UI.BuildingControlPanel.Part;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    [UpdateBefore(typeof(BuildingControlPanelSystem))]
    public partial class InitBuildingControlPanelSystem : SystemBase
    {
        private BuildingData _buildingData;
        private NativeList<ProductData> _required;
        private NativeList<ProductData> _manufactured;

        private BuildingControlPanelUI _buildingUI;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<InitializeTag>();
            RequireForUpdate<SelectedBuildingTag>();
        }

        protected override void OnUpdate()
        {
            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<InitializeTag, SelectedBuildingTag>())
            {
                SystemAPI
                    .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(World.Unmanaged)
                    .RemoveComponent<InitializeTag>(aspect.BuildingData.Self);

                _buildingUI = BuildingControlPanelUI.Instance;
                _buildingData = aspect.BuildingData;
                _required = aspect.RequiredProductsData.Value;
                _manufactured = aspect.ManufacturedProductsData.Value;

                #region Get data for set

                MainStorageBoxData mainStorageBoxData = SystemAPI.GetSingleton<MainStorageBoxData>();

                mainStorageBoxData.TryGetMatchingProducts(
                    _required,
                    out NativeList<ProductData> mainStorageProducts);

                BuildingProductsData productsInBuilding = aspect.ProductsInBuildingData;

                ProductData.ConvertProductsHashMapToList(
                    productsInBuilding.WarehouseBoxData.Value,
                    out var warehouseProducts);

                ProductData.ConvertProductsHashMapToList(
                    productsInBuilding.InProductionBoxData.Value,
                    out var inProductionBoxProducts);

                ProductData.ConvertProductsHashMapToList(
                    productsInBuilding.ManufacturedBoxData.Value,
                    out var manufacturedBoxProducts);

                #endregion

                #region Set data

                SetMainInfo();
                SetSpecsInfo();
                SetProductionLineInfo();

                SetItemsToMainStorage(mainStorageProducts);
                SetItemsToWarehouse(warehouseProducts);
                SetItemsToInProductionBox(inProductionBoxProducts);
                SetItemsToManufacturedBox(manufacturedBoxProducts);

                #endregion

                #region Dispose data after set

                mainStorageProducts.Dispose();
                warehouseProducts.Dispose();
                inProductionBoxProducts.Dispose();
                manufacturedBoxProducts.Dispose();

                #endregion
            }
        }

        #region Set init data to ui methods

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


        private void SetItemsToMainStorage(in NativeList<ProductData> mainStorageProducts) =>
            _buildingUI.SetItems(_buildingUI.StorageUI, in mainStorageProducts);

        private void SetItemsToWarehouse(NativeList<ProductData> warehouseProducts) =>
            _buildingUI.SetItems(_buildingUI.WarehouseUI, warehouseProducts);

        private void SetItemsToInProductionBox(NativeList<ProductData> inProductionBoxProducts) =>
            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionBoxProducts);

        private void SetItemsToManufacturedBox(NativeList<ProductData> manufacturedBoxProducts) =>
            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedBoxProducts);

        #endregion
    }
}