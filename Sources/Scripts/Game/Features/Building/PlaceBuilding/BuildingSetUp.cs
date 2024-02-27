using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Production;
using Sources.Scripts.Game.Common.SaveAndLoad;
using Sources.Scripts.Game.Features.Building.ControlPanel.Component;
using Sources.Scripts.Game.Features.Building.Events;
using Sources.Scripts.Game.Features.Building.PlaceBuilding.Component;
using Sources.Scripts.Game.Features.Building.Production.Component;
using Sources.Scripts.Game.Features.Building.Storage;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox.Component;
using Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox;
using Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox.Component;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding
{
    public class BuildingSetUp
    {
        private readonly RefRW<BuildingData> _building;
        private EntityCommandBuffer _bsEcb;
        private readonly Entity _entity;
        private BlueprintsBlobData _blobData;
        private readonly float3 _position;

        private readonly FixedString64Bytes _buildingName;

        private NativeList<ProductData> _requiredItems;
        private NativeList<ProductData> _manufacturedItems;
        private readonly NativeQueue<BuildingEvent> _buildingEvents;
        private readonly NativeQueue<ChangeProductsQuantityData> _changeProductsQuantityQueue;

        public BuildingSetUp(RefRW<BuildingData> buildingData, Entity entity, RefRO<LocalTransform> transform,
            BlueprintsBlobData blueprintsBlobData, EntityCommandBuffer ecb)
        {
            _building = buildingData;
            _entity = entity;
            _bsEcb = ecb;
            _blobData = blueprintsBlobData;
            _position = transform.ValueRO.Position;

            _buildingName = _building.ValueRO.NameId + "_" + _building.ValueRO.Guid;

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

        public unsafe void InitBuildingProductionData()
        {
            BuildingNameId buildingId = _building.ValueRO.NameId;

            _requiredItems = *_blobData.GetProductionLineProducts(buildingId).Required;
            _manufacturedItems = *_blobData.GetProductionLineProducts(buildingId).Manufactured;

            _bsEcb.AddComponent(_entity, new RequiredProductsData { Required = _requiredItems });
            _bsEcb.AddComponent(_entity, new ManufacturedProductsData { Manufactured = _manufacturedItems });
        }

        /// <summary>
        /// Set warehouse products quantity to 0 and production boxes data to 0 (in production/manufactured)
        /// </summary>
        public void InitBuildingProductsData()
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