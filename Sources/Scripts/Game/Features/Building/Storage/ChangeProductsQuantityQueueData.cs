using GamePlay.Features.Building.Storage.InProductionBox.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Features.Building.Storage
{
    public struct ChangeProductsQuantityQueueData : IComponentData
    {
        public NativeQueue<ChangeProductsQuantityData> Value;
    }
}