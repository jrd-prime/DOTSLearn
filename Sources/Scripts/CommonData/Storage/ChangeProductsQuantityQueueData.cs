using Sources.Scripts.CommonData.Storage.Data;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Storage
{
    public struct ChangeProductsQuantityQueueData : IComponentData
    {
        public NativeQueue<ChangeProductsQuantityData> Value;
    }
}