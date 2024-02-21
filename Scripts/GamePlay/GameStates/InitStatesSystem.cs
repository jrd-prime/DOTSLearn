using GamePlay.GameStates.BuildingState;
using GamePlay.GameStates.MainGameState;
using GamePlay.Shop.BlueprintsShop;
using GamePlay.Storage.MainStorage.Component;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.GameStates
{
    public partial class MyInitSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(MyInitSystemGroup))]
    public partial struct InitStatesSystem : ISystem
    {
        private static readonly FixedString64Bytes GameStateDataEntityName = "___ Game State";
        private static readonly FixedString64Bytes GameplayStateDataEntityName = "___ Data: Gameplay State";
        private static readonly FixedString64Bytes BuildingStateDataEntityName = "___ Data: Building State";
        private static readonly FixedString64Bytes BlueprintsShopDataName = "___ Data: Blueprints Shop";
        private static readonly FixedString64Bytes MainStorageDataName = "___ Data: Main Storage";

        public void OnUpdate(ref SystemState state)
        {
            Debug.LogWarning("Init Data");
            state.Enabled = false;

            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(1, Allocator.Temp)
            {
                { GameStateDataEntityName, typeof(GameStateData) },
                { GameplayStateDataEntityName, typeof(PlayStateData) },
                { BuildingStateDataEntityName, typeof(BuildingStateData) },
                { BlueprintsShopDataName, typeof(BlueprintsShopData) },
                { MainStorageDataName, typeof(MainStorageData) }
            };

            var entityManager = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var elementEntity = entityManager.CreateEntity(pair.Value);
                entityManager.SetName(elementEntity, pair.Key);
            }

            componentsMap.Dispose();
        }
    }
}