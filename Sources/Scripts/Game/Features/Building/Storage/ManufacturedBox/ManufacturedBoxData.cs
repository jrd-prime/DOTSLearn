using System;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox.Component;
using Unity.Collections;

namespace Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity)  of already produced products in the production box
    /// </summary>
    public struct ManufacturedBoxData : IBuildingProductsData
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