using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.Screen
{
    public struct ScreenData : IComponentData
    {
        public float2 WidthAndHeight;
        public float2 ScreenCenter;
    }
}