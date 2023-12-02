using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Grid.GridLayout
{
    public class GridAuthoring : MonoBehaviour
    {
        public int2 gridSize;
        public GameObject pointPrefabMain;
        public GameObject pointPrefabMid;
        public GameObject pointPrefabSmall;
    }

    public class GridLayoutBaker : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GridComponent
            {
                gridSize = authoring.gridSize,
                pointPrefabMain = GetEntity(authoring.pointPrefabMain, TransformUsageFlags.None),
                pointPrefabMid = GetEntity(authoring.pointPrefabMid, TransformUsageFlags.None),
                pointPrefabSmall = GetEntity(authoring.pointPrefabSmall, TransformUsageFlags.None)
            });
        }
    }
}