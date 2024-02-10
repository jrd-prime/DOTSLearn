using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Building.Production;
using Jrd.Gameplay.Products;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.MyUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Gameplay.Building.TempBuilding
{
    /// <summary>
    /// Place temp building prefab and init building info
    /// </summary>
    [BurstCompile]
    public partial struct PlaceTempBuildingSystem : ISystem
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
                _bsEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
                _guid = buildingData.ValueRO.Guid;
                _building = buildingData.ValueRW;
                _transform = transform.ValueRO;

                InitSettingsForNewBuilding(ref state);
            }
        }

        private void InitSettingsForNewBuilding(ref SystemState state)
        {
            Entity bufferEntity = SystemAPI.GetSingletonEntity<BuildingsPrefabsBufferTag>();

            var requiredItems = SystemAPI
                .GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
            var manufacturedItems = SystemAPI
                .GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

            NativeList<ProductData> required = GetProductionProductsList(requiredItems);
            NativeList<ProductData> manufactured = GetProductionProductsList(manufacturedItems);

            SetPosition();
            SetEntityName();
            AddBuildingTags();

            SetRequiredProductsData(required);
            SetManufacturedProductsData(manufactured);

            InitWarehouseData(required);
            InitProductionData(required, manufactured);

            AddBuildingToGameBuildingsList(ref state);
            RemoveTempTags();
        }

        private void SetPosition() => _building.WorldPosition = _transform.Position;
        private void SetEntityName() => _bsEcb.SetName(_entity, $"{_building.NameId}_{_guid}");

        private void AddBuildingTags()
        {
            _bsEcb.AddComponent<BuildingTag>(_entity);
            _bsEcb.AddComponent<AddBuildingToDBTag>(_entity);
        }

        /// <summary>
        /// Set component with required products list + required quantity
        /// </summary>
        private void SetRequiredProductsData(NativeList<ProductData> required)
        {
            _bsEcb.AddComponent(_entity, new RequiredProductsData
            {
                Required = required
            });
        }

        /// <summary>
        /// Set component with manufactured products list + required quantity
        /// </summary>
        private void SetManufacturedProductsData(NativeList<ProductData> manufactured)
        {
            _bsEcb.AddComponent(_entity, new ManufacturedProductsData
            {
                Manufactured = manufactured
            });
        }

        /// <summary>
        /// Set warehouse products quantity to 0
        /// </summary>
        private void InitWarehouseData(NativeList<ProductData> required)
        {
            NativeParallelHashMap<int, int> requiredProducts = Utils.NativeListToHashMap(required);

            _bsEcb.AddComponent(_entity, new WarehouseProductsData
            {
                Values = requiredProducts
            });
        }

        /// <summary>
        /// Set production boxes data to 0 (in production/manufactured)
        /// </summary>
        private void InitProductionData(NativeList<ProductData> required, NativeList<ProductData> manufactured)
        {
            _bsEcb.AddComponent(_entity, new InProductionData
            {
                Value = Utils.NativeListToHashMap(required)
            });

            _bsEcb.AddComponent(_entity, new ManufacturedData
            {
                Value = Utils.NativeListToHashMap(manufactured)
            });
        }

        private void AddBuildingToGameBuildingsList(ref SystemState _)
        {
            // add to buildings list for save mb
            NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsMap = SystemAPI
                .GetSingletonRW<GameBuildingsData>().ValueRW.GameBuildings;

            gameBuildingsMap.Add(_guid, _building);
        }

        private void InitRequiredAndManufacturedData(ref SystemState _,
            NativeList<ProductData> required,
            NativeList<ProductData> manufactured)
        {
            _bsEcb.AddComponent(_entity, new RequiredProductsData
            {
                Required = required
            });
            _bsEcb.AddComponent(_entity, new ManufacturedProductsData
            {
                Manufactured = manufactured
            });
        }


        private void RemoveTempTags()
        {
            _bsEcb.RemoveComponent<PlaceTempBuildingTag>(_entity);
            _bsEcb.RemoveComponent<TempBuildingTag>(_entity);
        }

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
    }
}