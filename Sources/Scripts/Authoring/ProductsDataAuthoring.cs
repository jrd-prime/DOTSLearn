using System;
using System.Collections.Generic;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.SO;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Authoring
{
    public class ProductsDataAuthoring : MonoBehaviour
    {
        [SerializeField] public List<ProductDataScriptable> _products;

        private class Baker : Baker<ProductsDataAuthoring>
        {
            public override void Bake(ProductsDataAuthoring authoring)
            {
                Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                DynamicBuffer<ProductsDataBuffer> buffer = AddBuffer<ProductsDataBuffer>(entity);

                foreach (var product in authoring._products)
                {
                    buffer.Add(new ProductsDataBuffer
                    {
                        Product = product.Product,
                        Name = product.Name,
                        PackSize = product.Size,
                        MoveTimeMultiplier = product.MoveTimeMultiplier
                    });
                }
            }
        }

        private void OnValidate()
        {
            foreach (var product in _products)
            {
                Assert.IsFalse(product.Name.Length <= 1, $"Check the SO {product.Name} name.");

                Assert.IsTrue(
                    product.Product.ToString().ToLower() == product.Name.ToLower().Replace(" ", string.Empty),
                    $"Check the SO {product.Name} category. ({product.Product}) not correspondence of these product");

                Assert.IsTrue(product.Size > 0, $"Check the SO {product.Name} pack size.");
            }
        }
    }
}