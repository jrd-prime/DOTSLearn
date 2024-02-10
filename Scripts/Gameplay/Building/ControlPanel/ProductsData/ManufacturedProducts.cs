using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage;
using Unity.Collections;

namespace Jrd.Gameplay.Building.ControlPanel.ProductsData
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity)  of already produced products in the production box
    /// </summary>
    public struct ManufacturedProducts : IBuildingProductsData
    {
        public NativeParallelHashMap<int, int> Value;

        public int GetProductQuantity(Product product)
        {
            throw new System.NotImplementedException();
        }

        public void SetProductsList(NativeParallelHashMap<int, int> productsMap) => Value = productsMap;
    }
}