using Grid.Points;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Grid.GridLayout
{
    public struct GridData : IComponentData
    {
        [SerializeField]
        public NativeList<PointComponent> PointData;
    }
}