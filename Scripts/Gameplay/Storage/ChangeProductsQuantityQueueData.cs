using GamePlay.Storage.InProductionBox.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Storage
{
    public struct ChangeProductsQuantityQueueData : IComponentData
    {
        public NativeQueue<ChangeProductsQuantityData> Value;
    }
}