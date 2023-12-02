using Unity.Entities;
using Unity.Mathematics;

namespace Grid.GridLayout
{
    public struct GridLayoutComponent : IComponentData
    {
        public int2 gridSize;
        public Entity pointPrefabMain;
        
        public Entity pointPrefabMid;
        public Entity pointPrefabSmall;
    }
}