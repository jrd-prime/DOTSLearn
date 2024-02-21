using GamePlay.Building.ControlPanel.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Building.SetUp
{
    public struct GameBuildingsData : IComponentData
    {
        public NativeHashMap<FixedString64Bytes, BuildingData> GameBuildings;
    }
}