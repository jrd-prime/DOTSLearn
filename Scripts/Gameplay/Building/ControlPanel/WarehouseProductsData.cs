using Jrd.Goods;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.Warehouse
{
    /// <summary>
    /// Building internal warehouse products list component
    /// </summary>
    public struct WarehouseProductsData : IComponentData
    {
        public NativeParallelHashMap<FixedString32Bytes, int> Values;
    }
}