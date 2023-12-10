using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Build.Screen
{
    /// <summary>
    /// Хранит координаты мира исходя из центра экрана
    /// </summary>
    public struct ScreenCenterInWorldCoordsComponent : IComponentData
    {
        public float3 ScreenCenterToWorld;
    }
}