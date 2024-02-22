using GamePlay.GameStates.BuildingState;
using GamePlay.GameStates.MainGameState;
using GamePlay.Prefabs;
using GamePlay.Shop.BlueprintsShop;
using GamePlay.Storage.MainStorage.Component;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.GameStates
{
    [UpdateInGroup(typeof(MyInitSystemGroup))]
    public partial struct InitStatesSystem : ISystem
    {
        private static readonly FixedString64Bytes GameStateDataEntityName = "___ Game State";
        private static readonly FixedString64Bytes GameplayStateDataEntityName = "___ Data: Gameplay State";
        private static readonly FixedString64Bytes BuildingStateDataEntityName = "___ Data: Building State";
        private static readonly FixedString64Bytes BlueprintsShopDataName = "___ Data: Blueprints Shop";
        private static readonly FixedString64Bytes MainStorageDataName = "___ Data: Main Storage";

        private DynamicBuffer<ProductsDataBuffer> _buffer;

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            if (!SystemAPI.TryGetSingletonBuffer<ProductsDataBuffer>(out var buffer))
            {
                Debug.LogError("NO Products Data Buffer!");
                return;
            }

            _buffer = buffer;

            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(0, Allocator.Temp)
            {
                { GameStateDataEntityName, typeof(GameStateData) },
                { GameplayStateDataEntityName, typeof(PlayStateData) },
                { BuildingStateDataEntityName, typeof(BuildingStateData) },
                { BlueprintsShopDataName, typeof(BlueprintsShopData) },
                { MainStorageDataName, typeof(MainStorageData) }
            };

            var em = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var entity = em.CreateEntity(pair.Value);

                em.SetName(entity, pair.Key);

                if (pair.Value == typeof(MainStorageData)) SetDefaultsToMainStorageData(entity, em);
            }

            componentsMap.Dispose();
        }

        private void SetDefaultsToMainStorageData(Entity elementEntity, EntityManager entityManager)
        {
            var mainStorageMap = new NativeParallelHashMap<int, int>(0, Allocator.Persistent);

            foreach (var buffer in _buffer)
            {
                mainStorageMap.Add((int)buffer.Product, -1);
            }

            entityManager.SetComponentData(elementEntity, new MainStorageData { Value = mainStorageMap });


            // const int all = 33;
            // entityManager.SetComponentData(elementEntity, new MainStorageData
            // {
            //     Value = new NativeParallelHashMap<int, int>(0, Allocator.Persistent)
            //     {
            //         { (int)Product.Wheat, all },
            //         { (int)Product.Flour, all },
            //         { (int)Product.Wood, all },
            //         { (int)Product.WoodenPlank, all },
            //         { (int)Product.Brick, all },
            //     }
            // });
        }
    }
}