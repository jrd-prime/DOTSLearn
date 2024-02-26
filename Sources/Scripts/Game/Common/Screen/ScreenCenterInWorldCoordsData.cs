using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.Game.Common.Screen
{
    /// <summary>
    /// Хранит координаты мира исходя из центра экрана
    /// </summary>
    public struct ScreenCenterInWorldCoordsData : IComponentData
    {
        public float3 ScreenCenterToWorld;
    }
}