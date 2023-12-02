using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Grid.GridLayout
{
    public class GridLayoutAuthoring : MonoBehaviour
    {
        public int2 gridSize;
        public GameObject pointPrefabMain;
        public GameObject pointPrefabMid;
        public GameObject pointPrefabSmall;
    }

    public class GridLayoutBaker : Baker<GridLayoutAuthoring>
    {
        public override void Bake(GridLayoutAuthoring authoring)
        {
            var _entity = GetEntity(TransformUsageFlags.None);
            AddComponent(_entity, new GridLayoutComponent
            {
                gridSize = authoring.gridSize,
                pointPrefabMain = GetEntity(authoring.pointPrefabMain, TransformUsageFlags.None),
                pointPrefabMid = GetEntity(authoring.pointPrefabMid, TransformUsageFlags.None),
                pointPrefabSmall = GetEntity(authoring.pointPrefabSmall, TransformUsageFlags.None)
            });
        }
    }
}