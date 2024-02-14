using System;
using Jrd.Gameplay.Building.ControlPanel.Component;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage;
using Jrd.Gameplay.Storage._1_MainStorage.Component;
using Jrd.Gameplay.Storage._2_Warehouse.Component;
using Jrd.Gameplay.Storage._3_InProduction.Component;
using Jrd.Gameplay.Storage._4_Manufactured;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
using Jrd.GameStates;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.PlayState;
using Jrd.MyUtils;
using Jrd.UI;
using Jrd.UI.BuildingControlPanel;
using Jrd.UI.BuildingControlPanel.Part;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.ControlPanel
{
    public partial class BuildingControlPanelSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton _sys;
        private EntityCommandBuffer _ecb;
        private Entity _buildingEntity;
        private BuildingDataAspect _aspect;
        private MainStorageData _mainStorageData;
        private WarehouseData _warehouseData;
        private InProductionData _inProductionData;
        private ManufacturedData _manufacturedData;
        private BuildingData _buildingData;
        private NativeList<ProductData> _required;
        private NativeList<ProductData> _manufactured;

        private BuildingControlPanelUI _buildingUI;
        private TextPopUpMono _textPopUpUI;

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
            _textPopUpUI = TextPopUpMono.Instance;

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

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<InitializeTag, SelectedBuildingTag>())
            {
                _aspect = aspect;
                _required = aspect.RequiredProductsData.Required;
                _manufactured = aspect.ManufacturedProductsData.Manufactured;
                _warehouseData = aspect.BuildingProductsData.WarehouseData;
                _inProductionData = aspect.BuildingProductsData.InProductionData;
                _manufacturedData = aspect.BuildingProductsData.ManufacturedData;
                _buildingData = aspect.BuildingData;
                _buildingEntity = aspect.BuildingData.Self;

                _ecb.RemoveComponent<InitializeTag>(_buildingEntity);

                SetMainInfo();
                SetSpecsInfo();
                SetProductionLineInfo();
                SetItemsToStorages();
                SetItemsToProduction();
            }

            // TODO LOOK REFACT THIS SH
            if (SystemAPI.HasComponent<UpdateStoragesDataTag>(_buildingEntity))
            {
                Debug.LogWarning("Update Storages Data Tag");
                _ecb.RemoveComponent<UpdateStoragesDataTag>(_buildingEntity);
            }

            if (SystemAPI.HasComponent<MainStorageDataUpdatedEvent>(_buildingEntity))
            {
                Debug.LogWarning("Main Storage Data Updated Event");
                SetItemsToMainStorage();
                _ecb.RemoveComponent<MainStorageDataUpdatedEvent>(_buildingEntity);
            }

            if (SystemAPI.HasComponent<WarehouseDataUpdatedEvent>(_buildingEntity))
            {
                Debug.LogWarning("Warehouse Data Updated Event");
                SetItemsToWarehouse();
                _ecb.RemoveComponent<WarehouseDataUpdatedEvent>(_buildingEntity);
            }

            if (SystemAPI.HasComponent<InProductionDataUpdatedEvent>(_buildingEntity))
            {
                Debug.LogWarning("In Production Data Updated Event");
                SetItemsToInProduction();
                _ecb.RemoveComponent<InProductionDataUpdatedEvent>(_buildingEntity);
            }

            if (SystemAPI.HasComponent<ManufacturedDataUpdatedEvent>(_buildingEntity))
            {
                Debug.LogWarning("Manufactured Data Updated Event");
                SetItemsToInProduction();
                _ecb.RemoveComponent<ManufacturedDataUpdatedEvent>(_buildingEntity);
            }

            foreach (var moveTimer in SystemAPI.Query<RefRO<ProductsMoveTimerData>>())
            {
                float timer = moveTimer.ValueRO.CurrentValue;
                float max = moveTimer.ValueRO.StarValue;

                switch (timer)
                {
                    case > 0f:
                        Debug.Log("Update timer UI");
                        SetStorageTimer(max, timer);
                        break;
                    case <= 0f:
                        Debug.Log("TIMER FINISHED");
                        SetStorageTimer(max, timer);
                        SetItemsToWarehouse();
                        break;
                }
            }
        }

        private void SetStorageTimer(float max, float value) => _buildingUI.SetTimerText(max, value);

        public void MoveButton()
        {
            _textPopUpUI.ShowPopUp("move btn");
            //TODO disable button if in storage 0 req products
            //TODO add move time to button

            _ecb.AddComponent<ProductsToWarehouseRequestTag>(_buildingEntity);
            _ecb.AddComponent<UpdateStoragesDataTag>(_buildingEntity);
        }

        public void LoadButton()
        {
            _textPopUpUI.ShowPopUp("load btn");

            _ecb.AddComponent<ProductsToProductionBoxRequestTag>(_buildingEntity);
            _ecb.AddComponent<UpdateStoragesDataTag>(_buildingEntity);
        }

        public void TakeButton()
        {
            Debug.LogWarning("take");
        }

        public void UpgradeButton()
        {
            Debug.LogWarning("upgrade");
        }

        public void BuffButton()
        {
            Debug.LogWarning("buff");
        }

        private void InstantDeliveryButton() => _ecb.AddComponent<InstantBuffTag>(_buildingEntity);

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

        private void SetItemsToStorages()
        {
            SetItemsToMainStorage();
            SetItemsToWarehouse();
        }

        private void SetItemsToWarehouse()
        {
            NativeList<ProductData> warehouseProductsList = StorageService.GetProductsDataList(_warehouseData.Value);

            _buildingUI.SetItems(_buildingUI.WarehouseUI, warehouseProductsList);

            warehouseProductsList.Dispose();
        }

        private void SetItemsToInProduction()
        {
            NativeList<ProductData> inProductionData = StorageService.GetProductsDataList(_inProductionData.Value);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionData);

            inProductionData.Dispose();
        }

        private void SetItemsToMainStorage()
        {
            NativeList<ProductData> mainStorageProductsList =
                StorageService.GetMatchingProducts(_required, _mainStorageData.Values, Allocator.Temp);

            _buildingUI.SetItems(_buildingUI.StorageUI, mainStorageProductsList);

            mainStorageProductsList.Dispose();
        }

        public void SetItemsToProduction()
        {
            var inProductionBox =
                Utils.ConvertProductsHashMapToList(_aspect.BuildingProductsData.InProductionData.Value);
            var manufacturedBox = Utils.ConvertProductsHashMapToList(
                _aspect.BuildingProductsData.ManufacturedData.Value);

            _buildingUI.SetItems(_buildingUI.InProductionUI, inProductionBox);
            _buildingUI.SetItems(_buildingUI.ManufacturedUI, manufacturedBox);
        }
    }
}