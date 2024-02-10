using Jrd.Gameplay.Building.Production;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers;
using Jrd.GameStates;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.PlayState;
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
        private Entity _entity;
        private MainStorageData _mainStorageData;
        private WarehouseProductsData _warehouseData;
        private ProductionData _productionData;
        private BuildingData _buildingData;


        private NativeList<ProductData> req;
        private NativeList<ProductData> man;

        private BuildingControlPanelUI _buildingUI;

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

            foreach (var (buildingData, warehouseProductsData, reqdata, mandata, inProductionData, manufacturedData,
                         entity) in SystemAPI
                         .Query<BuildingData, WarehouseProductsData, RequiredProductsData, ManufacturedProductsData,
                             InProductionData, ManufacturedData>()
                         .WithAll<InitializeTag, SelectedBuildingTag>()
                         .WithEntityAccess())
            {
                req = reqdata.Required;
                man = mandata.Manufactured;

                _warehouseData = warehouseProductsData;
                _buildingData = buildingData;
                _entity = entity;
                _ecb.RemoveComponent<InitializeTag>(entity);

                SetMainInfo();
                SetSpecsInfo();
                SetProductionLineInfo();
                SetItemsToStorages();
            }

            if (SystemAPI.HasComponent<UpdateStoragesDataTag>(_entity))
            {
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
            //TODO disable button if in storage 0 req products
            //TODO add move time to button

            _ecb.AddComponent<MoveRequestTag>(_entity);
            _ecb.AddComponent<UpdateStoragesDataTag>(_entity);
        }

        public void LoadButton()
        {
            Debug.LogWarning("load");
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
            _buildingUI.SetSpecName(Spec.Productivity,
                req.ElementAt(0).Name.ToString());
            _buildingUI.SetSpecName(Spec.LoadCapacity,
                req.ElementAt(0).Name.ToString());
            _buildingUI.SetSpecName(Spec.WarehouseCapacity,
                man.ElementAt(0).Name.ToString());

            _buildingUI.SetProductivity(_buildingData.ItemsPerHour);
            _buildingUI.SetLoadCapacity(_buildingData.LoadCapacity);
            _buildingUI.SetStorageCapacity(_buildingData.MaxStorage);
        }


        private void SetProductionLineInfo() => _buildingUI.SetLineInfo(req, man);

        private void SetItemsToStorages()
        {
            SetItemsToMainStorage();
            SetItemsToWarehouse();
        }

        private void SetItemsToWarehouse()
        {
            NativeList<ProductData> warehouseProductsList = _warehouseData.GetProductsList(req);

            _buildingUI.SetItems(_buildingUI.WarehouseUI, warehouseProductsList);

            warehouseProductsList.Dispose();
        }

        private void SetItemsToMainStorage()
        {
            NativeList<ProductData> mainStorageProductsList = _mainStorageData.GetMatchingProducts(req);

            _buildingUI.SetItems(_buildingUI.StorageUI, mainStorageProductsList);

            mainStorageProductsList.Dispose();
        }

        public void SetItemsToProduction()
        {
            _buildingUI.SetItems(_buildingUI.InProductionBoxUI, _productionData.ProductsIn);
            _buildingUI.SetItems(_buildingUI.ManufacturedBoxUI, _productionData.ProductsOut);
        }
    }
}