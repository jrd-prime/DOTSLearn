using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building;
using Sources.Scripts.Game.Features.Building.ControlPanel;
using Sources.Scripts.Game.Features.Building.ControlPanel.Component;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox.Component;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage.Component;
using Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox;
using Sources.Scripts.Game.Features.Building.Storage.Service;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse.Component;
using Sources.Scripts.UI;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UI.BuildingControlPanel.Part;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using BuildingDataAspect = Sources.Scripts.Game.Features.Building.BuildingDataAspect;

namespace Sources.Scripts.Game.InitSystems
{
    [UpdateBefore(typeof(BuildingControlPanelSystem))]
    public partial class InitBuildingSystem : SystemBase
    {
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
        private BeginSimulationEntityCommandBufferSystem.Singleton _sys;

        private BuildingControlPanelUI _buildingUI;
        private TextPopUpMono _textPopUpUI;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<InitializeTag>();
            RequireForUpdate<SelectedBuildingTag>();
        }

        protected override void OnStartRunning()
        {
            _sys = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            _buildingUI = BuildingControlPanelUI.Instance;
            _textPopUpUI = TextPopUpMono.Instance;
        }

        protected override void OnUpdate()
        {
            _ecb = _sys.CreateCommandBuffer(World.Unmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<InitializeTag, SelectedBuildingTag>())
            {
                _aspect = aspect;
                _required = aspect.RequiredProductsData.Required;
                _manufactured = aspect.ManufacturedProductsData.Manufactured;
                _warehouseData = aspect.BuildingProductsData.WarehouseData;
                _inProductionBoxData = aspect.BuildingProductsData.InProductionBoxData;
                _manufacturedBoxData = aspect.BuildingProductsData.ManufacturedBoxData;
                _buildingData = aspect.BuildingData;
                _buildingEntity = aspect.BuildingData.Self;


                foreach (var q in _required)
                {
                    Debug.LogWarning($"req {q.Name} / {q.Quantity}");
                }

                foreach (var q in _manufactured)
                {
                    Debug.LogWarning($"man {q.Name} / {q.Quantity}");
                }


                _ecb.RemoveComponent<InitializeTag>(_buildingEntity);

                SetMainInfo();
                SetSpecsInfo();
                SetProductionLineInfo();
                SetItemsToMainStorage();
                SetItemsToWarehouse();
                SetItemsToProduction();
            }
        }

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
                StorageService.GetMatchingProducts(_required, _mainStorageData.Value, out bool isEnough);

            _buildingUI.SetItems(_buildingUI.StorageUI, mainStorageProductsList);

            mainStorageProductsList.Dispose();
        }

        public void SetItemsToProduction()
        {
            var inProductionBox =
                ProductData.ConvertProductsHashMapToList(_aspect.BuildingProductsData.InProductionBoxData.Value);
            var manufacturedBox = ProductData.ConvertProductsHashMapToList(
                _aspect.BuildingProductsData.ManufacturedBoxData.Value);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionBox);
            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedBox);
        }
    }
}