using System;
using GamePlay.GameStates.BuildingState;
using GamePlay.GameStates.MainGameState;
using GamePlay.Prefabs;
using GamePlay.Shop.BlueprintsShop;
using GamePlay.Storage.MainStorage.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.GameStates
{
    [UpdateInGroup(typeof(MyInitSystemGroup))]
    public partial struct InitStatesSystem : ISystem
    {
        private DynamicBuffer<ProductsDataBuffer> _buffer;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlueprintsBlobData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            if (!SystemAPI.TryGetSingletonBuffer(out _buffer)) throw new Exception($"NO {nameof(ProductsDataBuffer)}!");

            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(0, Allocator.Temp)
            {
                { Const.Names.GameStateDataEntityName, typeof(GameStateData) },
                { Const.Names.GameplayStateDataEntityName, typeof(PlayStateData) },
                { Const.Names.BuildingStateDataEntityName, typeof(BuildingStateData) },
                { Const.Names.BlueprintsShopDataName, typeof(BlueprintsShopData) },
                { Const.Names.MainStorageDataName, typeof(MainStorageData) }
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