using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers;
using Jrd.GameStates;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.PlayState;
using Jrd.UI.BuildingControlPanel;
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
        private BuildingData _buildingData;
        private DynamicBuffer<BuildingRequiredItemsBuffer> _requiredItems;
        private DynamicBuffer<BuildingManufacturedItemsBuffer> _manufacturedItems;

        private BuildingControlPanelUI _controlPanelUI;

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<JBuildingsPrefabsTag>();
        }

        protected override void OnStartRunning()
        {
            _sys = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            _controlPanelUI = BuildingControlPanelUI.Instance;

            _controlPanelUI.MoveButton.clicked += MoveButton;
            _controlPanelUI.LoadButton.clicked += LoadButton;
            _controlPanelUI.TakeButton.clicked += TakeButton;
            _controlPanelUI.UpgradeButton.clicked += UpgradeButton;
            _controlPanelUI.BuffButton.clicked += BuffButton;
        }

        protected override void OnUpdate()
        {
            _ecb = _sys.CreateCommandBuffer(World.Unmanaged);

            var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
            // cache mb somewhere
            _requiredItems = SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
            _manufacturedItems = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

            foreach (var (buildingData, warehouseProductsData, entity) in SystemAPI
                         .Query<BuildingData, WarehouseProductsData>()
                         .WithAll<InitializeTag, SelectedBuildingTag>()
                         .WithEntityAccess())
            {
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
                        Debug.Log("Update timer UI");
                        SetStorageTimer(max, timer);
                        break;
                    case <= 0.5f:
                        // show timer finished

                        Debug.Log("TIMER FINISHED");
                        Debug.Log("Update storages UI");
                        SetItemsToStorages();
                        _ecb.RemoveComponent<ProductsMoveTimerData>(_entity);
                        break;
                }
            }
        }

        private void SetStorageTimer(float max, float value) => _controlPanelUI.SetTimerText(max, value);

        public void MoveButton()
        {
            //TODO disable button if in storage 0 req products
            Debug.LogWarning("move");

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

        private void SetMainInfo()
        {
            _controlPanelUI.SetLevel(_buildingData.Level);
        }

        private void SetSpecsInfo()
        {
            // TODO refact
            _controlPanelUI.SetSpecName(Spec.Productivity,
                _manufacturedItems.ElementAt(0)._item.ToString());
            _controlPanelUI.SetSpecName(Spec.LoadCapacity,
                _requiredItems.ElementAt(0)._item.ToString());
            _controlPanelUI.SetSpecName(Spec.WarehouseCapacity,
                _manufacturedItems.ElementAt(0)._item.ToString());

            _controlPanelUI.SetProductivity(_buildingData.ItemsPerHour);
            _controlPanelUI.SetLoadCapacity(_buildingData.LoadCapacity);
            _controlPanelUI.SetStorageCapacity(_buildingData.MaxStorage);
        }


        private void SetProductionLineInfo() => _controlPanelUI.SetLineInfo(_requiredItems, _manufacturedItems);

        private void SetItemsToStorages()
        {
            SetItemsToMainStorage();
            SetItemsToWarehouse();
        }

        private void SetItemsToWarehouse()
        {
            NativeList<ProductData> warehouseProductsList =
                _warehouseData.GetProductsList(_requiredItems);

            _controlPanelUI.SetWarehouseItems(warehouseProductsList);
            warehouseProductsList.Dispose();
        }

        private void SetItemsToMainStorage()
        {
            NativeList<ProductData> mainStorageProductsList =
                _mainStorageData.GetMatchingProducts(_requiredItems);

            _controlPanelUI.SetStorageItems(mainStorageProductsList);
            mainStorageProductsList.Dispose();
        }
    }
}