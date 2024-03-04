using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonComponents.test
{
    public struct ChangeProductsQuantityQueueData : IComponentData
    {
        public NativeQueue<ChangeProductsQuantityData> Value;
    }
}