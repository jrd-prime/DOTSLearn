using Jrd.Goods;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.MainStorage
{
    public struct MainStorageData : IComponentData, IDB
    {
        public NativeParallelHashMap<FixedString32Bytes, int> Values;

        public int GetProductCount(GoodsEnum product)
        {
            FixedString32Bytes key = nameof(product);

            return Values.ContainsKey(key) ? Values[key] : -1;
        }
    }

    public interface IDB
    {
        public int GetProductCount(GoodsEnum product);
    }
}