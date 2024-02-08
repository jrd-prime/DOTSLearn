using System;
using UnityEngine.Serialization;

namespace Jrd.Gameplay.Products
{
    [Serializable]
    public struct ProductionProductData
    {
        [FormerlySerializedAs("_product")] public Product _productName;
        public int _quantity;
    }
}