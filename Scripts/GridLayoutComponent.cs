using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace DefaultNamespace
{
    public struct GridLayoutComponent : IComponentData
    {
        public int2 gridSize;
        public Entity pointPrefabMain;
        
        public Entity pointPrefabMid;
        public Entity pointPrefabSmall;
    }
}