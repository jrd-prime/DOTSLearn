using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Build
{
    public struct PlaceBuildingComponent: IComponentData
    {
        public float3 placePosition;
    }
}