﻿using CommonComponents.Building;
using CommonComponents.Product;
using GamePlay.Common.SaveAndLoad;
using GamePlay.Features.Building.ControlPanel.Component;
using GamePlay.Features.Building.Events;
using GamePlay.Features.Building.PlaceBuilding.Component;
using GamePlay.Features.Building.Production;
using GamePlay.Features.Building.Production.Component;
using GamePlay.Features.Building.Products.Component;
using GamePlay.Features.Building.Storage;
using GamePlay.Features.Building.Storage.InProductionBox.Component;
using GamePlay.Features.Building.Storage.ManufacturedBox;
using GamePlay.Features.Building.Storage.ManufacturedBox.Component;
using GamePlay.Features.Building.Storage.Warehouse.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace GamePlay.Features.Building.PlaceBuilding
{
    /// <summary>
    /// Place temp building prefab and init building info
    /// </summary>
    [BurstCompile]
    public partial struct SetUpBuildingSystem : ISystem
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;

        private Entity _entity;
        private FixedString64Bytes _guid;
        private BuildingData _building;

        // Dispose
        private NativeList<ProductData> _requiredItems;
        private NativeList<ProductData> _manufacturedItems;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlueprintsBlobData>();
            state.RequireForUpdate<PlaceTempBuildingTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
            _requiredItems.Dispose();
            _manufacturedItems.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

            foreach (var (buildingData, transform, entity) in SystemAPI
                         .Query<RefRW<BuildingData>, RefRO<LocalTransform>>()
                         .WithAll<PlaceTempBuildingTag, TempBuildingTag>()
                         .WithEntityAccess())
            {
                _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

                _entity = entity;
                _building = buildingData.ValueRW;
                _guid = _building.Guid;

                buildingData.ValueRW.Self = entity;
                buildingData.ValueRW.WorldPosition = transform.ValueRO.Position;
                buildingData.ValueRW.BuildingEvents = new NativeQueue<BuildingEvent>(Allocator.Persistent);

                BuildingNameId buildingId = _building.NameId;
                BlueprintsBlobData blueprintsBlobData = SystemAPI.GetSingleton<BlueprintsBlobData>();

                _requiredItems = blueprintsBlobData.GetProductionLineProducts(buildingId).Required;
                _manufacturedItems = blueprintsBlobData.GetProductionLineProducts(buildingId).Manufactured;

                InitSettingsForNewBuilding(ref state);
            }
        }

        [BurstCompile]
        private void InitSettingsForNewBuilding(ref SystemState state)
        {
            // Main
            // SetPosition();
            SetBuildingEntityName();
            SetProductionState();
            InitChangeProductsQuantityQueue();
            BuildingTags();

            // Init products
            InitBuildingProductsData();
            SetRequiredProductsData();
            SetManufacturedProductsData();
            SetProductionProcessDataComponent();

            // Save?
            AddBuildingToGameBuildingsList(ref state);
        }

        private void InitChangeProductsQuantityQueue()
        {
            _bsEcb.AddComponent(_entity, new ChangeProductsQuantityQueueData
            {
                Value = new NativeQueue<ChangeProductsQuantityData>(Allocator.Persistent)
            });
        }

        private void BuildingTags()
        {
            _bsEcb.AddComponent<BuildingTag>(_entity);
            _bsEcb.AddComponent<AddBuildingToDBTag>(_entity);

            _bsEcb.RemoveComponent<PlaceTempBuildingTag>(_entity);
            _bsEcb.RemoveComponent<TempBuildingTag>(_entity);
        }

        /// <summary>
        /// Set warehouse products quantity to 0 and production boxes data to 0 (in production/manufactured)
        /// </summary>
        private void InitBuildingProductsData()
        {
            _bsEcb.AddComponent(_entity, new BuildingProductsData
            {
                WarehouseData = new WarehouseData
                    { Value = ProductData.ConvertProductsDataToHashMap(_requiredItems, ProductValues.ToDefault) },
                InProductionBoxData = new InProductionBoxData
                    { Value = ProductData.ConvertProductsDataToHashMap(_requiredItems, ProductValues.ToDefault) },
                ManufacturedBoxData = new ManufacturedBoxData
                    { Value = ProductData.ConvertProductsDataToHashMap(_manufacturedItems, ProductValues.ToDefault) }
            });
        }

        private void SetBuildingEntityName() => _bsEcb.SetName(_entity, _building.NameId + "_" + _guid);
        private void SetProductionState() => _building.ProductionState = ProductionState.Init;
        private void SetProductionProcessDataComponent() => _bsEcb.AddComponent<ProductionProcessData>(_entity);

        /// <summary>
        /// Set component with required products list + required quantity
        /// </summary>
        private void SetRequiredProductsData() =>
            _bsEcb.AddComponent(_entity, new RequiredProductsData { Required = _requiredItems });

        /// <summary>
        /// Set component with manufactured products list + required quantity
        /// </summary>
        private void SetManufacturedProductsData() =>
            _bsEcb.AddComponent(_entity, new ManufacturedProductsData { Manufactured = _manufacturedItems });

        private void AddBuildingToGameBuildingsList(ref SystemState _)
        {
            // add to buildings list for save mb
            NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsMap = SystemAPI
                .GetSingletonRW<GameBuildingsMapData>().ValueRW.GameBuildings;

            gameBuildingsMap.Add(_guid, _building);
        }
    }
}