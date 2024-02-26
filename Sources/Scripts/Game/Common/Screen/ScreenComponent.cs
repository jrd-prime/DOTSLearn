using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.Game.Common.Screen
{
    public struct ScreenComponent : IComponentData
    {
        public float2 WidthAndHeight;
        public float2 ScreenCenter;
    }
}