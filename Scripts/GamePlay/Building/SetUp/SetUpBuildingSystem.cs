using GamePlay.Building.ControlPanel.Component;
using GamePlay.Building.Production;
using GamePlay.Building.Production.Component;
using GamePlay.Building.SetUp.Component;
using GamePlay.Prefabs;
using GamePlay.Products.Component;
using GamePlay.Storage;
using GamePlay.Storage.InProductionBox.Component;
using GamePlay.Storage.ManufacturedBox;
using GamePlay.Storage.ManufacturedBox.Component;
using GamePlay.Storage.Warehouse.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GamePlay.Building.SetUp
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

                buildingData.ValueRW.BuildingEvents = new NativeQueue<BuildingEvent>(Allocator.Persistent);

                InitSettingsForNewBuilding(ref state);
            }
        }

        [BurstCompile]
        private void InitSettingsForNewBuilding(ref SystemState state)
        {
            Entity bufferEntity = SystemAPI.GetSingletonEntity<BuildingsPrefabsBufferTag>();

            var requiredItems = SystemAPI
                .GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);


            foreach (var q in requiredItems)
            {
                Debug.LogWarning($"req in {q.Value.Name} / {q.Value.Quantity} / {this}");
            }


            var manufacturedItems = SystemAPI
                .GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

            NativeList<ProductData> required = GetProductionProductsList(requiredItems);
            NativeList<ProductData> manufactured = GetProductionProductsList(manufacturedItems);


            // Main
            SetPosition();
            SetEntityName();
            SetProductionState();
            InitChangeProductsQueue();
            BuildingTags();

            // Init products

            // foreach (var q in manufactured)
            // {
            //     Debug.LogWarning("manufactured set = " + q.Name + " / " + q.Quantity);
            // }


            InitBuildingProductsData(required, manufactured);
            SetRequiredProductsData(required);
            SetManufacturedProductsData(manufactured);

            SetProductionProcessDataComponent();

            // Save?
            AddBuildingToGameBuildingsList(ref state);
        }

        private void InitChangeProductsQueue()
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
        private void InitBuildingProductsData(NativeList<ProductData> required, NativeList<ProductData> manufactured)
        {
            _bsEcb.AddComponent(_entity, new BuildingProductsData
            {
                WarehouseData = new WarehouseData
                    { Value = ProductData.ConvertProductsDataToHashMap(required, ProductValues.ToDefault) },
                InProductionBoxData = new InProductionBoxData
                    { Value = ProductData.ConvertProductsDataToHashMap(required, ProductValues.ToDefault) },
                ManufacturedBoxData = new ManufacturedBoxData
                    { Value = ProductData.ConvertProductsDataToHashMap(manufactured, ProductValues.ToDefault) }
            });
        }

        private void SetPosition() => _building.WorldPosition = _transform.Position;
        private void SetEntityName() => _bsEcb.SetName(_entity, $"{_building.NameId}_{_guid}");
        private void SetProductionState() => _building.ProductionState = ProductionState.Init;
        private void SetProductionProcessDataComponent() => _bsEcb.AddComponent<ProductionProcessData>(_entity);

        /// <summary>
        /// Set component with required products list + required quantity
        /// </summary>
        private void SetRequiredProductsData(NativeList<ProductData> required) =>
            _bsEcb.AddComponent(_entity, new RequiredProductsData { Required = required });


        /// <summary>
        /// Set component with manufactured products list + required quantity
        /// </summary>
        private void SetManufacturedProductsData(NativeList<ProductData> manufactured) =>
            _bsEcb.AddComponent(_entity, new ManufacturedProductsData { Manufactured = manufactured });

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
            DynamicBuffer<BuildingProductionItemsBuffer> productsBuffer = buffer
                .Reinterpret<BuildingProductionItemsBuffer>();

            NativeList<ProductData> productsList = new(productsBuffer.Length, Allocator.Persistent);

            foreach (var product in productsBuffer)
            {
                productsList.Add(product.Value);
            }

            return productsList;
        }

        #endregion
    }
}