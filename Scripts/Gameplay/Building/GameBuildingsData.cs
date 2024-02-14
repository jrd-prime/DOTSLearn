using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Building.ControlPanel.Component;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Building
{
    public struct GameBuildingsData : IComponentData
    {
        public NativeHashMap<FixedString64Bytes, BuildingData> GameBuildings;
    }
}