using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Production;
using Sources.Scripts.CommonComponents.test;
using Sources.Scripts.Game.Features.Building.ControlPanel;
using Sources.Scripts.Game.Features.Building.Events;
using Sources.Scripts.Game.Features.Building.Storage;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse;
using Sources.Scripts.SaveAndLoad;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding
{
    public struct BuildingSetUpDataWrapper
    {
        public RefRW<BuildingData> BuildingData;
        public Entity Entity;
        public RefRO<LocalTransform> Transform;
        public NativeList<ProductData> RequiredItemsList;
        public NativeList<ProductData> ManufacturedItemsList;
        public EntityCommandBuffer Ecb;
    }

    public struct BuildingSetUp
    {
        private EntityCommandBuffer _bsEcb;
        private readonly Entity _entity;
        private readonly RefRW<BuildingData> _building;
        private readonly float3 _position;

        private readonly FixedString64Bytes _buildingName;

        private readonly NativeList<ProductData> _requiredItemsList;
        private readonly NativeList<ProductData> _manufacturedItemsList;
        private readonly NativeQueue<BuildingEvent> _buildingEvents;
        private readonly NativeQueue<ChangeProductsQuantityData> _changeProductsQuantityQueue;

        public BuildingSetUp(BuildingSetUpDataWrapper buildingSetUpData)
        {
            _building = buildingSetUpData.BuildingData;
            _entity = buildingSetUpData.Entity;
            _bsEcb = buildingSetUpData.Ecb;
            _requiredItemsList = buildingSetUpData.RequiredItemsList;
            _manufacturedItemsList = buildingSetUpData.ManufacturedItemsList;
            _position = buildingSetUpData.Transform.ValueRO.Position;

            _buildingName = $"{_building.ValueRO.NameId}_{_building.ValueRO.Guid}";

            _buildingEvents = new NativeQueue<BuildingEvent>(Allocator.Persistent);
            _changeProductsQuantityQueue = new NativeQueue<ChangeProductsQuantityData>(Allocator.Persistent);
        }

        public void Initialize()
        {
            RemoveBuildingTags();

            SetMainInfo();

            InitBuildingEventsQueue();
            InitBuildingProductionData();
            InitBuildingProductsData();
            InitChangeProductsQuantityQueue();

            SetProductionState();
            SetProductionProcessDataComponent();

            AddBuildingTags();
        }

        private void SetMainInfo()
        {
            _building.ValueRW.Self = _entity;
            _building.ValueRW.WorldPosition = _position;
            _bsEcb.SetName(_entity, _buildingName);
        }

        public void InitBuildingProductionData()
        {
            _bsEcb.AddComponent(_entity, new RequiredProductsData { Value = _requiredItemsList });
            _bsEcb.AddComponent(_entity, new ManufacturedProductsData { Value = _manufacturedItemsList });
        }

        /// <summary>
        /// Set warehouse products quantity to 0 and production boxes data to 0 (in production/manufactured)
        /// </summary>
        public void InitBuildingProductsData()
        {
            _bsEcb.AddComponent(_entity, new BuildingProductsData
            {
                WarehouseData = new WarehouseData
                {
                    Value = ProductData.ConvertProductsDataToHashMap(_requiredItemsList, ProductValues.ToDefault)
                },
                InProductionBoxData = new InProductionBoxData
                {
                    Value = ProductData.ConvertProductsDataToHashMap(_requiredItemsList, ProductValues.ToDefault)
                },
                ManufacturedBoxData = new ManufacturedBoxData
                {
                    Value = ProductData.ConvertProductsDataToHashMap(_manufacturedItemsList,
                        ProductValues.ToDefault)
                }
            });
        }

        public void InitChangeProductsQuantityQueue() =>
            _bsEcb.AddComponent(_entity, new ChangeProductsQuantityQueueData { Value = _changeProductsQuantityQueue });

        private void AddBuildingTags() =>
            _bsEcb.AddComponent(_entity, new ComponentTypeSet(
                typeof(BuildingTag),
                typeof(AddBuildingToDBTag)
            ));

        private void RemoveBuildingTags() =>
            _bsEcb.RemoveComponent(_entity, new ComponentTypeSet(
                typeof(PlaceTempBuildingTag),
                typeof(TempBuildingTag)
            ));

        public void SetProductionState() => _building.ValueRW.ProductionState = ProductionState.Init;
        public void SetProductionProcessDataComponent() => _bsEcb.AddComponent<ProductionProcessData>(_entity);
        public void InitBuildingEventsQueue() => _building.ValueRW.BuildingEvents = _buildingEvents;

        // private void AddBuildingToGameBuildingsList(ref SystemState _)
        // {
        //     // add to buildings list for save mb
        //     NativeHashMap<FixedString64Bytes, BuildingData> gameBuildingsMap = SystemAPI
        //         .GetSingletonRW<GameBuildingsMapData>().ValueRW.GameBuildings;
        //
        //     gameBuildingsMap.Add(_guid, _building);
        // }
    }
}