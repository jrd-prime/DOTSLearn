using Grid.Points;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Grid.GridLayout
{
    public struct GridData : IComponentData
    {
        public NativeList<PointComponent> PointsData;
    }
}