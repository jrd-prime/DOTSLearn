using System;
using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage.Data;
using Sources.Scripts.Game.Features.Shop.BlueprintsShop;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = System.Random;

namespace Sources.Scripts.Game.InitSystems
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct InitDataSystem : ISystem
    {
        private DynamicBuffer<ProductsDataBuffer> _buffer;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlueprintsBlobAssetReference>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            if (!SystemAPI.TryGetSingletonBuffer(out _buffer)) throw new Exception($"NO {nameof(ProductsDataBuffer)}!");

            {
                // Assert
                // TODO add output of missing products
                var enumProductsLength = Enum.GetNames(typeof(Product)).Length;
                var bufferLength = _buffer.Length;

                Assert.IsTrue(enumProductsLength == bufferLength,
                    $"The enum {typeof(Product)} quantity ({enumProductsLength}) " +
                    $"does not match the quantity ({bufferLength}) in {typeof(ProductsDataBuffer)} (Products Data Authoring)." +
                    $"Check {typeof(Product)} or products in products authoring.");
            }

            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(0, Allocator.Temp)
            {
                // { Names.GameStateDataEntityName, typeof(GameStateData) },
                // { Names.GameplayStateDataEntityName, typeof(PlayStateData) },
                { Names.BuildingStateDataEntityName, typeof(BuildingStateData) },
                { Names.BlueprintsShopDataName, typeof(BlueprintsShopData) },
                { Names.MainStorageDataName, typeof(MainStorageBoxData) }
            };

            var em = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var entity = em.CreateEntity(pair.Value);

                em.SetName(entity, pair.Key);

                if (pair.Value == typeof(MainStorageBoxData)) SetDefaultsToMainStorageData(entity, em);
            }

            componentsMap.Dispose();
        }

        private void SetDefaultsToMainStorageData(Entity elementEntity, EntityManager entityManager)
        {
            var mainStorageMap = new NativeParallelHashMap<int, int>(0, Allocator.Persistent);

            // foreach (var buffer in _buffer)
            // {
            //     mainStorageMap.Add((int)buffer.Product, -1);
            // }
            //
            // entityManager.SetComponentData(elementEntity, new MainStorageData { Value = mainStorageMap });

            var a = new Random();
            foreach (var buffer in _buffer)
            {
                mainStorageMap.Add((int)buffer.Product, a.Next(10, 20));
            }

            entityManager.SetComponentData(elementEntity, new MainStorageBoxData { Value = mainStorageMap });
        }
    }
}