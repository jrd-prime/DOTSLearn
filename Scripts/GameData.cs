using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Jrd
{
    // TODO временно
    public struct GameData : IComponentData
    {
        public NativeList<int> a;
        // private NativeHashMap<float3, EData> b;/

    }
}