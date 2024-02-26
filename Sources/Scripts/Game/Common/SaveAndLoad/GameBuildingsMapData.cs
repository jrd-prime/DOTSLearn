using Sources.Scripts.Game.Features.Building.ControlPanel.Component;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Common.SaveAndLoad
{
    public struct GameBuildingsMapData : IComponentData
    {
        public NativeHashMap<FixedString64Bytes, BuildingData> GameBuildings;
    }
}