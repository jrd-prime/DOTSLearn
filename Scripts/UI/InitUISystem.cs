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
        private static readonly FixedString64Bytes BuildingPanel = "___ Building Panel";
        private static readonly FixedString64Bytes ConfirmationPanel = "___ Confirmation Panel";
        private static readonly FixedString64Bytes MainStoragePanel = "___ Main Storage Panel";

        public void OnCreate(ref SystemState state)
        {
            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(1, Allocator.Temp)
            {
                { BuildingPanel, typeof(BlueprintsShopPanelData) },
                { ConfirmationPanel, typeof(ConfirmationPanelData) },
                { MainStoragePanel, typeof(MainStorageData) }
            };

            var entityManager = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var elementEntity = entityManager.CreateEntity(pair.Value, typeof(UIElementTag));
                entityManager.SetName(elementEntity, pair.Key);
                Debug.Log($"Init: {pair.Key}");
            }

            componentsMap.Dispose();
        }
    }
}