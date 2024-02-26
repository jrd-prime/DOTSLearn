using Sources.Scripts.Grid.Points;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Grid.GridLayout
{
    public struct GridData : IComponentData
    {
        public NativeList<PointComponent> PointsData;
    }
}