using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.UserInput
{
    public struct InputCursorData : IComponentData
    {
        public float3 CursorWorldPosition;
        public float3 CursorScreenPosition;
        public CursorState CursorState;
        public Ray ClickToRay;
    }
}