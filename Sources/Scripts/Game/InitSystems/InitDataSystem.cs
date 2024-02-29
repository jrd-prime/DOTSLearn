using System;
using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage;
using Sources.Scripts.Game.Features.Shop.BlueprintsShop;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.InitSystems
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct InitDataSystem : ISystem
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
                // { Names.GameStateDataEntityName, typeof(GameStateData) },
                // { Names.GameplayStateDataEntityName, typeof(PlayStateData) },
                { Names.BuildingStateDataEntityName, typeof(BuildingStateData) },
                { Names.BlueprintsShopDataName, typeof(BlueprintsShopData) },
                { Names.MainStorageDataName, typeof(MainStorageData) }
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

            // foreach (var buffer in _buffer)
            // {
            //     mainStorageMap.Add((int)buffer.Product, -1);
            // }
            //
            // entityManager.SetComponentData(elementEntity, new MainStorageData { Value = mainStorageMap });

            var a = new Random();
            
            foreach (var buffer in _buffer)
            {
                mainStorageMap.Add((int)buffer.Product, a.Next(0, 55));
            }
            
            entityManager.SetComponentData(elementEntity, new MainStorageData { Value = mainStorageMap });
        }
    }
}