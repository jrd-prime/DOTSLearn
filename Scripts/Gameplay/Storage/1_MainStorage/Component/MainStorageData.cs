using Jrd.Gameplay.Products;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage._1_MainStorage.Component
{
    public struct MainStorageData : IComponentData, IMainStorage
    {
        /// <summary>
        /// Hash map (int <see cref="Product"/> id, int quantity of product)
        /// </summary>
        public NativeParallelHashMap<int, int> Values;
    }
}