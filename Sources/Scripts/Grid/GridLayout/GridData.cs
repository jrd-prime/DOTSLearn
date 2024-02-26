using Jrd.Grid.Points;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Grid.GridLayout
{
    public struct GridData : IComponentData
    {
        public NativeList<PointComponent> PointsData;
    }
}