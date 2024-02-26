using Sources.Scripts.Game.Features.Building.Storage.InProductionBox.Component;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage
{
    public struct ChangeProductsQuantityQueueData : IComponentData
    {
        public NativeQueue<ChangeProductsQuantityData> Value;
    }
}