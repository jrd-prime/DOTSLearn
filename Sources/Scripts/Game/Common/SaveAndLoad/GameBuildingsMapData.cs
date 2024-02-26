using GamePlay.Features.Building.ControlPanel.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Common.SaveAndLoad
{
    public struct GameBuildingsMapData : IComponentData
    {
        public NativeHashMap<FixedString64Bytes, BuildingData> GameBuildings;
    }
}