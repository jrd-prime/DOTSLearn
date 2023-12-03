using Unity.Entities;
using Unity.Mathematics;

namespace UserInput
{
    public struct CursorComponent : IComponentData
    {
        public float3 cursorPosition;
    }
}