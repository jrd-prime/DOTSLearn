using System;
using CommonComponents.Product;
using GamePlay.Features.Building.Products.Component;
using Unity.Collections;

namespace GamePlay.Features.Building.Storage.InProductionBox.Component
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity) of the products in ready-to-process
    /// </summary>
    public struct InProductionBoxData : IBuildingProductsData
    {
        public NativeParallelHashMap<int, int> Value;

        // TODO repeated, needed refact
        public void ChangeProductsQuantity(ChangeType change, NativeList<ProductData> productsData)
        {
            foreach (var product in productsData)
            {
                ChangeProductQuantity(product, change);
            }
        }

        private void ChangeProductQuantity(ProductData product, ChangeType change)
        {
            switch (change)
            {
                case ChangeType.Increase:
                    Value[(int)product.Name] += product.Quantity;
                    break;
                case ChangeType.Reduce:
                    Value[(int)product.Name] -= product.Quantity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change), change, null);
            }
        }
    }
}