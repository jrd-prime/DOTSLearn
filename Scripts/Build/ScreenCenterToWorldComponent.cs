using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Build
{
    /// <summary>
    /// Хранит координаты мира исходя из центра скрина
    /// </summary>
    public struct ScreenCenterToWorldComponent : IComponentData
    {
        public float3 ScreenCenterToWorld;
    }
}