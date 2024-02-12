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

        private bool IsWarehouseHasRequiredQuantityProducts(NativeParallelHashMap<int, int> warehouseCurrentProducts,
            NativeList<ProductData> requiredQuantity)
        {
            var a = false;

            foreach (var q in requiredQuantity)
            {
                if (warehouseCurrentProducts[(int)q.Name] >= q.Quantity)
                {
                    Debug.LogWarning(
                        $"product {q.Name} in warehouse {warehouseCurrentProducts[(int)q.Name]} and reqQuantity = {q.Quantity}");
                }
                else
                {
                    Debug.LogWarning(
                        $"product {q.Name} in warehouse {warehouseCurrentProducts[(int)q.Name]} and reqQuantity = {q.Quantity}");
                }
            }


            return a;
        }
    }
}