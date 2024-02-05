using Jrd.Gameplay.Storage.MainStorage;
using Jrd.GameStates.BuildingState.BuildingPanel;
using Jrd.GameStates.BuildingState.ConfirmationPanel;
using Jrd.Goods;
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
                    Values = new NativeParallelHashMap<FixedString32Bytes, int>(1, Allocator.Persistent)
                    {
                        { nameof(GoodsEnum.Wheat), 0 },
                        { nameof(GoodsEnum.Flour), 0 },
                        { nameof(GoodsEnum.Wood), 0 },
                        { nameof(GoodsEnum.WoodenPlank), -1 },
                        { nameof(GoodsEnum.Brick), 0 },
                    }
                });
            }

            componentsMap.Dispose();
        }
    }
}