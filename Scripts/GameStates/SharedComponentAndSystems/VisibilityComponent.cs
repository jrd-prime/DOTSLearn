﻿using Unity.Entities;

namespace Jrd.GameStates.BuildingState
{
    public struct VisibilityComponent : IComponentData
    {
        public bool IsVisible;
    }
}