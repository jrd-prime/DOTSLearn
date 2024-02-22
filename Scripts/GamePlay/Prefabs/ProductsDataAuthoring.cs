using System.Collections.Generic;
using GamePlay.Products;
using GamePlay.ScriptableObjects;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.Prefabs
{
    public class ProductsDataAuthoring : MonoBehaviour
    {
        [SerializeField] public List<ProductDataSO> _products;

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
                        PackSize = product.Size,
                        MoveTimeMultiplier = product.MoveTimeMultiplier
                    });
                }
            }
        }
    }

    public struct ProductsDataBuffer : IBufferElementData
    {
        public Product Product;
        public int PackSize;
        public float MoveTimeMultiplier;
    }
}