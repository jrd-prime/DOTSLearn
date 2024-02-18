using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Timers;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.Production
{
    public struct ProductionManager
    {
        private readonly BuildingDataAspect _buildingData;
        private Entity _buildingEntity;

        public ProductionManager(BuildingDataAspect buildingData)
        {
            _buildingData = buildingData;
            _buildingEntity = buildingData.Self;
        }

    }
}