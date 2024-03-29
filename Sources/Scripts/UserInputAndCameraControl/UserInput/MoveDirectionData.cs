﻿using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.UserInputAndCameraControl.UserInput
{
    /// <summary>
    /// Component with direction from user input
    /// <remarks>An entity with this component is movable from user input</remarks>
    /// </summary>
    public struct MoveDirectionData : IComponentData
    {
        public float3 Direction;
    }
}