using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.UserInput
{
    public struct CursorComponent : IComponentData
    {
        public float3 cursorPosition;
    }
}