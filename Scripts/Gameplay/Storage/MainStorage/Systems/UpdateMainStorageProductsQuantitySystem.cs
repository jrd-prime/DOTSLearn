using System;
using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage.MainStorage.Systems
{
    /// <summary>
    /// Increase or reduce the quantity of products in the main storage<br />
    /// <para>By tags: <see cref="IncreaseMainStorageProductsTag"/> and <see cref="ReduceMainStorageProductsTag"/></para>
    /// <para>Required <see cref="ProductsToDeliveryData"/></para>
    /// </summary>
    [BurstCompile]
    public partial struct UpdateMainStorageProductsQuantitySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToDeliveryData>();
        }

        private void A(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            MainStorageData mainStorage = SystemAPI.GetSingleton<MainStorageData>();

            foreach (var (aspect, products) in SystemAPI
                         .Query<BuildingDataAspect, ProductsToDeliveryData>()
                         .WithAll<IncreaseMainStorageProductsTag>())
            {
                Debug.Log("Increase main storage prods");
                mainStorage.IncreaseProductsQuantityByKey(products.Value);

                ecb.AddComponent<MainStorageDataUpdatedEvent>(aspect.Self);

                ecb.RemoveComponent<IncreaseMainStorageProductsTag>(aspect.Self);
            }

            foreach (var (aspect, products) in SystemAPI
                         .Query<BuildingDataAspect, ProductsToDeliveryData>()
                         .WithAll<ReduceMainStorageProductsTag>())
            {
                Debug.Log("Reduce main storage prods");
                mainStorage.ReduceProductsQuantityByKey(products.Value);

                ecb.AddComponent<MainStorageDataUpdatedEvent>(aspect.Self);

                ecb.RemoveComponent<ReduceMainStorageProductsTag>(aspect.Self);
            }
        }
    }
}