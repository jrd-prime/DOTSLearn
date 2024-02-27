using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.Screen
{
    /// <summary>
    /// Contains data [in world coords] from screen center
    /// </summary>
    public struct ScreenCenterInWorldCoordsData : IComponentData
    {
        public float3 Value;
    }
}