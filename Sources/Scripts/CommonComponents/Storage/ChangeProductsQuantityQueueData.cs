using Sources.Scripts.CommonComponents.Storage.Data;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Storage
{
    public struct ChangeProductsQuantityQueueData : IComponentData
    {
        public NativeQueue<ChangeProductsQuantityData> Value;
    }
}