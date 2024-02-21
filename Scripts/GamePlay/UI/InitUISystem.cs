using GamePlay.Products;
using GamePlay.Shop.BlueprintsShop;
using GamePlay.Storage.MainStorage.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.UI
{
    /// <summary>
    /// List UI Components to create entities for them
    /// </summary>
    public partial struct InitUISystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(0, Allocator.Temp);


            var entityManager = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var elementEntity = entityManager.CreateEntity(pair.Value, typeof(UIElementTag));
                entityManager.SetName(elementEntity, pair.Key);

                // add test data to main storage
                // if (pair.Key != MainStorageDataName) continue;
                // entityManager.AddComponent<UpdateRequestTag>(elementEntity);
                entityManager.AddComponent<MainStorageData>(elementEntity);
                var all = 33;
                entityManager.SetComponentData(elementEntity, new MainStorageData
                {
                    Value = new NativeParallelHashMap<int, int>(0, Allocator.Persistent)
                    {
                        { (int)Product.Wheat, all },
                        { (int)Product.Flour, all },
                        { (int)Product.Wood, all },
                        { (int)Product.WoodenPlank, all },
                        { (int)Product.Brick, all },
                    }
                });
            }

            componentsMap.Dispose();
        }
    }
}