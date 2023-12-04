﻿using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Grid.GridLayout
{
    public struct GridComponent : IComponentData
    {
        public int2 gridSize;
        public Entity pointPrefabMain;
        public Entity pointPrefabMid;
        public Entity pointPrefabSmall;
        public bool findPoints;
    }
}