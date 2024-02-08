using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Products;
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
    public partial struct PlaceTempBuildingSystem : ISystem
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _bsEcb;
        private Entity _entity;
        private FixedString64Bytes _guid;
        private BuildingData _building;

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
                buildingData.ValueRW.WorldPosition = transform.ValueRO.Position;

                _guid = buildingData.ValueRO.Guid;
                _building = buildingData.ValueRO;

                Entity bufferEntity = SystemAPI.GetSingletonEntity<BuildingsPrefabsBufferTag>();

                var requiredItems = SystemAPI
                    .GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
                var manufacturedItems = SystemAPI
                    .GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

                NativeList<ProductionProductData> required = GetProductionProductsList(requiredItems);
                NativeList<ProductionProductData> manufactured = GetProductionProductsList(manufacturedItems);

                AddMainComponents();
                AddBuildingTags();
                InitProductionData(ref state, required, manufactured);
                InitWarehouseData(required);
                AddBuildingToGameBuildingsList(ref state);
                RemoveTempTags();

                //TODO add here tag for add building to db
            }
        }

        private void AddBuildingToGameBuildingsList(ref SystemState _)
        {
            // add to buildings list for save mb
            NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsMap = SystemAPI
                .GetSingletonRW<GameBuildingsData>().ValueRW.GameBuildings;

            gameBuildingsMap.Add(_guid, _building);
        }

        private void InitProductionData(ref SystemState _,
            NativeList<ProductionProductData> required,
            NativeList<ProductionProductData> manufactured)
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

        private void AddMainComponents()
        {
            _bsEcb.SetName(_entity, $"{_building.NameId}_{_guid}");
            _bsEcb.AddComponent<BuildingTag>(_entity);
        }

        /// <summary>
        /// Set warehouse products init data (required products with 0 quantity
        /// </summary>
        private void InitWarehouseData(NativeList<ProductionProductData> required)
        {
            NativeParallelHashMap<int, int> requiredProducts = Utils.NativeListToHashMap(required);

            _bsEcb.AddComponent(_entity, new WarehouseProductsData
            {
                Values = requiredProducts
            });
        }

        private void AddBuildingTags()
        {
            _bsEcb.AddComponent<AddBuildingToDBTag>(_entity);
        }

        private void RemoveTempTags()
        {
            _bsEcb.RemoveComponent<PlaceTempBuildingTag>(_entity);
            _bsEcb.RemoveComponent<TempBuildingTag>(_entity);
        }

        private NativeList<ProductionProductData> GetProductionProductsList<T>(
            DynamicBuffer<T> buffer) where T : unmanaged, IBufferElementData
        {
            DynamicBuffer<BuildingProductionItemsBuffer> productsBuffer =
                buffer.Reinterpret<BuildingProductionItemsBuffer>();
            NativeList<ProductionProductData> productsList = new(productsBuffer.Length, Allocator.Persistent);

            foreach (var product in productsBuffer)
            {
                productsList.Add(product.Value);
            }

            return productsList;
        }
    }
}