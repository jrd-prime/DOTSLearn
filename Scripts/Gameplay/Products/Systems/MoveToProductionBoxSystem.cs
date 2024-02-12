using System;
using Jrd.Gameplay.Building;
using Jrd.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Products
{
    public partial struct MoveToProductionBoxSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToProductionBoxRequestTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<ProductsToProductionBoxRequestTag>())
            {
                Debug.LogWarning("to production box request");

                NativeParallelHashMap<int, int> warehouseProducts =
                    aspect.BuildingProductsData.WarehouseProductsData.Value;

                NativeList<ProductData> requiredQuantity = aspect.RequiredProductsData.Required;


                if (IsWarehouseHasRequiredQuantityProducts(warehouseProducts, requiredQuantity))
                {
                    TextPopUpMono.Instance.ShowPopUp("quantity ok");
                }
                else
                {
                    TextPopUpMono.Instance.ShowPopUp("quantity NOT ok");
                }

                ecb.RemoveComponent<ProductsToProductionBoxRequestTag>(aspect.BuildingData.Self);
            }
        }

        private bool IsWarehouseHasRequiredQuantityProducts(NativeParallelHashMap<int, int> warehouseProducts,
            NativeList<ProductData> requirements)
        {
            // Delete
            foreach (var q in requirements)
            {
                if (warehouseProducts[(int)q.Name] >= q.Quantity)
                {
                    Debug.LogWarning(
                        $"product {q.Name} in warehouse {warehouseProducts[(int)q.Name]} and reqQuantity = {q.Quantity}");
                }
                else
                {
                    Debug.LogWarning(
                        $"product {q.Name} in warehouse {warehouseProducts[(int)q.Name]} and reqQuantity = {q.Quantity}");
                }
            }

            bool first = warehouseProducts[(int)requirements[0].Name] >= requirements[0].Quantity;

            //TODO LOOK Refactor this 
            return requirements.Length switch
            {
                0 => throw new Exception("Building without requirements!!! OMG!!!"),
                1 => first,
                2 => first && warehouseProducts[(int)requirements[1].Name] >= requirements[1].Quantity,
                _ => false
            };
        }
    }
}