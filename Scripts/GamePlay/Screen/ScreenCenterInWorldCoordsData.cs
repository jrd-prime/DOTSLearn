using Unity.Entities;
using Unity.Mathematics;

namespace GamePlay.Screen
{
    /// <summary>
    /// Хранит координаты мира исходя из центра экрана
    /// </summary>
    public struct ScreenCenterInWorldCoordsData : IComponentData
    {
        public float3 ScreenCenterToWorld;
    }
}