using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage;
using Unity.Collections;

namespace Jrd.Gameplay.Building.ControlPanel.ProductsData
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity) of the products in ready-to-process
    /// </summary>
    public struct InProductionProducts : IBuildingProductsData
    {
        public NativeParallelHashMap<int, int> Value;
        public int GetProductQuantity(Product product)
        {
            throw new System.NotImplementedException();
        }
        
        
        public void SetProductsList(NativeParallelHashMap<int, int> productsMap) => Value = productsMap;
    }
}