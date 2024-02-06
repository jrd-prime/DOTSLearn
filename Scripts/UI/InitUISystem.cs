using Jrd.Gameplay.Product;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.UI
{
    /// <summary>
    /// List UI Components to create entities for them
    /// </summary>
    public partial struct InitUISystem : ISystem
    {
        private static readonly FixedString64Bytes BlueprintsShopDataName = "___ Data: Blueprints Shop";
        private static readonly FixedString64Bytes ConfirmationPanel = "___ Confirmation Panel";
        private static readonly FixedString64Bytes MainStorageDataName = "___ Data: Main Storage";

        public void OnCreate(ref SystemState state)
        {
            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(1, Allocator.Temp)
            {
                { BlueprintsShopDataName, typeof(BlueprintsShopData) }, //LOOK not UI
                { ConfirmationPanel, typeof(ConfirmationPanelData) }, // need data?
                { MainStorageDataName, typeof(MainStorageData) } //LOOK not UI
            };

            var entityManager = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var elementEntity = entityManager.CreateEntity(pair.Value, typeof(UIElementTag));
                entityManager.SetName(elementEntity, pair.Key);

                // add test data to main storage
                if (pair.Key != MainStorageDataName) continue;
                entityManager.AddComponent<UpdateRequestTag>(elementEntity);
                entityManager.AddComponent<MainStorageData>(elementEntity);
                entityManager.SetComponentData(elementEntity, new MainStorageData
                {
                    Values = new NativeParallelHashMap<int, int>(1, Allocator.Persistent)
                    {
                        { (int)Product.Wheat, 10 },
                        { (int)Product.Flour, 20 },
                        { (int)Product.Wood, 30 },
                        { (int)Product.WoodenPlank, 40 },
                        { (int)Product.Brick, 50 },
                    }
                });
            }

            componentsMap.Dispose();
        }
    }
}