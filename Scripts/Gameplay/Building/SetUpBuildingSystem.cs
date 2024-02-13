﻿using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Building.ControlPanel.ProductsData;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.MyUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Jrd.Gameplay.Building.TempBuilding
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
        private LocalTransform _transform;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingsPrefabsBufferTag>();
            state.RequireForUpdate<PlaceTempBuildingTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
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
                _entity = entity;
                buildingData.ValueRW.Self = entity;
                _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
                _guid = buildingData.ValueRO.Guid;
                _building = buildingData.ValueRW;
                _transform = transform.ValueRO;

                InitSettingsForNewBuilding(ref state);
            }
        }

        [BurstCompile]
        private void InitSettingsForNewBuilding(ref SystemState state)
        {
            Entity bufferEntity = SystemAPI.GetSingletonEntity<BuildingsPrefabsBufferTag>();

            var requiredItems = SystemAPI
                .GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
            var manufacturedItems = SystemAPI
                .GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

            NativeList<ProductData> required = GetProductionProductsList(requiredItems);
            NativeList<ProductData> manufactured = GetProductionProductsList(manufacturedItems);

            // Main
            SetPosition();
            SetEntityName();
            BuildingTags();

            // Init products
            InitBuildingProductsData(required, manufactured);
            SetRequiredProductsData(required);
            SetManufacturedProductsData(manufactured);

            // Save?
            AddBuildingToGameBuildingsList(ref state);
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
        private void InitBuildingProductsData(NativeList<ProductData> required, NativeList<ProductData> manufactured)
        {
            _bsEcb.AddComponent(_entity, new BuildingProductsData
            {
                WarehouseProductsData = SetDefaultData<WarehouseProducts>(required),
                InProductionData = SetDefaultData<InProductionProducts>(required),
                ManufacturedData = SetDefaultData<ManufacturedProducts>(manufactured)
            });
        }

        private void SetPosition() => _building.WorldPosition = _transform.Position;
        private void SetEntityName() => _bsEcb.SetName(_entity, $"{_building.NameId}_{_guid}");

        /// <summary>
        /// Set component with required products list + required quantity
        /// </summary>
        private void SetRequiredProductsData(NativeList<ProductData> required)
        {
            _bsEcb.AddComponent(_entity, new RequiredProductsData { Required = required });
        }

        /// <summary>
        /// Set component with manufactured products list + required quantity
        /// </summary>
        private void SetManufacturedProductsData(NativeList<ProductData> manufactured)
        {
            _bsEcb.AddComponent(_entity, new ManufacturedProductsData { Manufactured = manufactured });
        }

        private void AddBuildingToGameBuildingsList(ref SystemState _)
        {
            // add to buildings list for save mb
            NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsMap = SystemAPI
                .GetSingletonRW<GameBuildingsData>().ValueRW.GameBuildings;

            gameBuildingsMap.Add(_guid, _building);
        }

        #region Local Utils

        /// <summary>
        /// Contains list of <see cref="ProductData"/> with product and quantity from building required/manufactured buffer
        /// </summary>
        private NativeList<ProductData> GetProductionProductsList<T>(
            DynamicBuffer<T> buffer) where T : unmanaged, IBufferElementData
        {
            DynamicBuffer<BuildingProductionItemsBuffer> productsBuffer =
                buffer.Reinterpret<BuildingProductionItemsBuffer>();
            NativeList<ProductData> productsList = new(productsBuffer.Length, Allocator.Persistent);

            foreach (var product in productsBuffer)
            {
                productsList.Add(product.Value);
            }

            return productsList;
        }

        /// <summary>
        /// Convert list <see cref="ProductData"/> to hashmap, set <see cref="IBuildingProductsData"/> data quantity to 0
        /// </summary>
        private T SetDefaultData<T>(NativeList<ProductData> list) where T : IBuildingProductsData, new()
        {
            NativeParallelHashMap<int, int> productsMap =
                Utils.ConvertProductsDataToHashMap(list, Utils.ProductValues.ToDefault);

            T productsData = new();
            productsData.SetProductsList(productsMap);

            return productsData;
        }

        #endregion
    }
}