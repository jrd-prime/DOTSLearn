using Jrd.Gameplay.Products;
using Jrd.Gameplay.Shop.BlueprintsShop;
using Jrd.Gameplay.Storage.MainStorage.Component;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.UI
{
    /// <summary>
    /// List UI Components to create entities for them
    /// </summary>
    public partial struct InitUISystem : ISystem
    {
        private static readonly FixedString64Bytes BlueprintsShopDataName = "___ Data: Blueprints Shop";
        private static readonly FixedString64Bytes MainStorageDataName = "___ Data: Main Storage";

        public void OnCreate(ref SystemState state)
        {
            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(1, Allocator.Temp)
            {
                { BlueprintsShopDataName, typeof(BlueprintsShopData) }, //LOOK not UI
                { MainStorageDataName, typeof(MainStorageData) } //LOOK not UI
            };

            var entityManager = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var elementEntity = entityManager.CreateEntity(pair.Value, typeof(UIElementTag));
                entityManager.SetName(elementEntity, pair.Key);

                // add test data to main storage
                if (pair.Key != MainStorageDataName) continue;
                // entityManager.AddComponent<UpdateRequestTag>(elementEntity);
                entityManager.AddComponent<MainStorageData>(elementEntity);
                entityManager.SetComponentData(elementEntity, new MainStorageData
                {
                    Value = new NativeParallelHashMap<int, int>(0, Allocator.Persistent)
                    {
                        { (int)Product.Wheat, 0 },
                        { (int)Product.Flour, 20 },
                        { (int)Product.Wood, 1 },
                        { (int)Product.WoodenPlank, 40 },
                        { (int)Product.Brick, 50 },
                    }
                });
            }

            componentsMap.Dispose();
        }
    }
}