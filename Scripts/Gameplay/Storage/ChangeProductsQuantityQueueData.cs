using Jrd.Gameplay.Storage.InProductionBox.Component;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage
{
    public struct ChangeProductsQuantityQueueData : IComponentData
    {
        public NativeQueue<ChangeProductsQuantityData> Value;
    }
}