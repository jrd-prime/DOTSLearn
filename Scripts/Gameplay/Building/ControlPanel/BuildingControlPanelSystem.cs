using Jrd.Gameplay.Building.ControlPanel.ProductsData;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers;
using Jrd.GameStates;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.PlayState;
using Jrd.UI;
using Jrd.UI.BuildingControlPanel;
using Jrd.UI.BuildingControlPanel.Part;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utils = Jrd.MyUtils.Utils;

namespace Jrd.Gameplay.Building.ControlPanel
{
    public partial class BuildingControlPanelSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton _sys;
        private EntityCommandBuffer _ecb;
        private Entity _entity;
        private MainStorageData _mainStorageData;

        private BuildingDataAspect _aspect;
        private WarehouseProducts _warehouseData;
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
                _warehouseData = aspect.BuildingProductsData.WarehouseProductsData;
                _buildingData = aspect.BuildingData;
                _entity = aspect.BuildingData.Self;

                _ecb.RemoveComponent<InitializeTag>(_entity);

                SetMainInfo();
                SetSpecsInfo();
                SetProductionLineInfo();
                SetItemsToStorages();
                SetItemsToProduction();
            }

            if (SystemAPI.HasComponent<UpdateStoragesDataTag>(_entity))
            {
                Debug.LogWarning("Update Storages Data Tag>");
                _ecb.RemoveComponent<UpdateStoragesDataTag>(_entity);
            }

            foreach (var moveTimer in SystemAPI.Query<RefRO<ProductsMoveTimerData>>())
            {
                float timer = moveTimer.ValueRO.CurrentValue;
                float max = moveTimer.ValueRO.StarValue;

                switch (timer)
                {
                    case > 0:
                        // TODO refact
                        Debug.Log("Update timer UI");
                        SetStorageTimer(max, timer);
                        break;
                    case <= 0.5f:
                        // show timer finished

                        Debug.Log("TIMER FINISHED");
                        Debug.Log("Update storages UI");
                        SetStorageTimer(max, timer);
                        SetItemsToStorages();
                        _ecb.RemoveComponent<ProductsMoveTimerData>(_entity);
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

            _ecb.AddComponent<ProductsToWarehouseRequestTag>(_entity);
            _ecb.AddComponent<UpdateStoragesDataTag>(_entity);
        }

        public void LoadButton()
        {
            _textPopUpUI.ShowPopUp("load btn");
            
            _ecb.AddComponent<ProductsToProductionBoxRequestTag>(_entity);
            _ecb.AddComponent<UpdateStoragesDataTag>(_entity);
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

        private void InstantDeliveryButton() => _ecb.AddComponent<InstantBuffTag>(_entity);

        private void SetMainInfo()
        {
            _buildingUI.SetLevel(_buildingData.Level);
        }

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
            NativeList<ProductData> warehouseProductsList = _warehouseData.GetProductsDataList();

            _buildingUI.SetItems(_buildingUI.WarehouseUI, warehouseProductsList);

            warehouseProductsList.Dispose();
        }

        private void SetItemsToMainStorage()
        {
            NativeList<ProductData> mainStorageProductsList = _mainStorageData.GetMatchingProducts(_required);

            _buildingUI.SetItems(_buildingUI.StorageUI, mainStorageProductsList);

            mainStorageProductsList.Dispose();
        }

        public void SetItemsToProduction()
        {
            var inProductionBox =
                Utils.ConvertProductsHashMapToList(_aspect.BuildingProductsData.InProductionData.Value);
            var manufacturedBox = Utils.ConvertProductsHashMapToList(
                _aspect.BuildingProductsData.ManufacturedData.Value);

            _buildingUI.SetItems(_buildingUI.InProductionBoxUI, inProductionBox);
            _buildingUI.SetItems(_buildingUI.ManufacturedBoxUI, manufacturedBox);
        }
    }
}